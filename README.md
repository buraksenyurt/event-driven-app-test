# Event-Driven Yaklaşımlı .Net Çözümlerinde Test Edilebilirlik

Bu repoya konu olan çalışmadaki amaç .Net ile yazılmış ve event-driven yaklaşımını benimsemiş çözümlerde uçtan uca karmaşık testlerin nasıl yapılacağını anlamaya çalışmaktır. Microservice yaklaşımını benimseyen dağıtık sistem çözümlerinde test edilebilirlik önemlidir. Sistemin parçalarının her an test edilebilir olması code coverage değerlerini yukarı çekip olası üretim hatalarının önüne geçmek için elzemdir. Repodaki çalışmada amaç öncelikle Event-Driven yaklaşıma uygun basit bir çözüm geliştirmek sonrasında test konusunu ele almaktır.

## Senaryolar

Çözüm içerisinde iki servis yer alıyor. EmployeeService ve InsuranceService. Her ikisi de basit birer REST servisi ve kendi SQLite veritabanlarına sahipler. InsuranceService içerisinde yer alan controller sınıfı, kontrat oluşturmak, güncellemek ve listelemek için üç fonksiyon içeriyor. EmployeeService içerisinde yer alan controller'da çalışan için satacağı poliçe sayısını hazırlamak, çalışan ve kontratları listelemek için üç fonksiyon içermekte. Yeni bir kontrat oluşturulması veya güncellenmesi EmployeeService için bir event anlamı taşımakta ve bu değişiklikler EmployeeService tarafından da değerlendirilmekte. Benzer durum sistemdeki bir sigortacı(çalışan) satışa çıkarken kullanacağı poliçileri sepetine aldığı zaman da geçerli. Bu durumda InsuranceService bundan haberdar olabilmeli ve kontrat miktarları buna göre revize edilmeli. 

Servislerde gerçekleşen bazı eylemler sistem için bir olay(event) anlamına gelmekte. Bu noktada bir olayın muhatabının bu olayla ilgilenmesi için bir sisteme ihtiyaç var. RabbitMQ burada devreye giriyor. Servisler olaylarını asenkron olarak RabittMQ kuyruğuna bırakabiliyorlar. Olayın muhatapları bu aktiviteleri dinleyip kendi tarafları için gerekli eylemleri yapabiliyorlar. Bir servis tarafından üretilen bir nesnenin diğer servis tarafından da anlaşılabilmesi için ideal bir yapı. Dağıtık sistemlerde çok kullanılan bir çözüm.

## RabbitMQ Hazırlıkları

RabbitMQ'yu sistemde kolayca ayağa kaldırmak için DockerCompose dosyasından yararlanılabilir.

```shell
sudo docker-compose up
```

RabbitMQ sistemde ayağa kalktıktan sonra varsayılan olarak http://localhost:15672/#/ adresinden ulaşılabilir. Çözümde servisler arası haberleşmede bir exchange topic ve ayrılmış mesaj kuyrukları söz konusu.

Oluşturulan sales.exchange isimli exchange nesnesinin özellikleri şöyle olmalı.

```text
Name : sales.exchange
Type : topic
Durable : true,
AutoDelete : false,
Internal : false,
Arguments : null
```

Bu exchange ile ilişkili olan kuyruklar ise aşağıdaki gibi oluşturulmalı.

```text
Name : insurance.employee
Type : classic
VHost: /
AutoDelete : false
Durable : true

Name : insurance.contract
Type : classic
VHost: /
AutoDelete : false
Durable : true
```

Oluşturulan bu mesaj kuyruklarının ilgili exchange'e bağlanması da gerekir. Aşağıdaki ekran görüntüleri setup konusunda fikir verebilir.

![assets/rabbitmq_01.png](assets/rabbitmq_01.png)

![assets/rabbitmq_02.png](assets/rabbitmq_02.png)

![assets/rabbitmq_03.png](assets/rabbitmq_03.png)

### RabbitMQ Çalışırlığının Kontrolü

Eğer işler yolunda gittiyse örnek bir Contract oluşturma talebine karşılık RabbitMQ tarafındaki ilgili mesaj kuyruğunda bir hareket olması gerekir.

Örneğin InsuranceService ayağa kaldırıldıktan sonra yeni bir sigorta sözleşme tipini sisteme eklemek istediğimizde aşağıdaki gibi bir talep kullanabiliriz.

```bash
curl -X 'POST' \
  'http://localhost:5011/Contract' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
  "contractId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Bireyse Sağlık Sigortası Tip A",
  "quantity": 10
}'
```

Buna karşın RabbitMQ tarafında şöyle bir şeyler görebiliyor olmamız gerekir.

![assets/rabbitmq_04.png](assets/rabbitmq_04.png)

![assets/rabbitmq_05.png](assets/rabbitmq_05.png)

## İlk Gün Testi

InsuranceService'teki CreateContract servis çağrımı ile yeni bir poliçe sisteme girildiğinde RabbitMQ tarafındaki insurance.contract kuyruğuna yeni eklenen veri bilgisini içeren bir mesaj bırakılır. EmployeeService ayaktaysa eğer, background task olarak çalışan servis insurance.contract kuyruğuna gelen mesajı yakalar. Mesaj içeriğinde gelen JSON tabanlı Contract içeriğini değerlendirir. ContractId bilgisini kullanarak kendi veritabanında _(EmployeeService.db)_ böyle bir poliçe olup olmadığına bakar. Eğer varsa kendi veritabanındaki poliçe bilgilerini günceller, yoksa yeni bir poliçe olarak ekler.

Bu ilk çalışma zamanı testini gerçekleştirmek için EmployeeService ve InsuranceService uygulamaları ayrı ayrı başlatılır. RabbitMQ arabirimine de http://localhost:15672 adresinden erişilir olduğundan emin olmak gerekir. İlk gün denemesinde insurance.contract isimli kuyruğa akan mesajlar yakalanır. Örneğin aşağıdaki ekran görüntüsünde olduğu gibi InsuranceService üstünden yeni bir Contract oluşturulduğunda, EmployeeService uygulaması buna ilişkin üretilen olayı dinlediği mesaj kuyruğundan yakalar, gerekli EF komutlarını işletir.

![assets/rabbitmq_06.png](assets/rabbitmq_06.png)

ve hatta bu otomatik gerçekleşen dinleme işlemi sonrası EmployeeService üstünden poliçe bilgileri istendiğinde aşağıdaki gibi yeni gelen poliçenin kendi veritabanına eklendiği gözlemlenir.

![assets/rabbitmq_07.png](assets/rabbitmq_07.png)

## İkinci Gün Testi

EmployeeService'indeki Post fonksiyonunu kullanarak var olan bir poliçenin miktarını değiştirdiğimizde buna bağlı olarak Insurance tarafındaki sözleşmenin de miktarı değiştirilir. Yani EmployeeService bir olay ile InsuranceService'i uyarırken tam tersi yönde bir mesaj yayını da söz konusudur. Örneğin EmployeeService üzerinden aşağıdaki gibi bir çağrım gerçekleştirdiğimizde,

![assets/rabbitmq_08.png](assets/rabbitmq_08.png)

InsuranceService tarafında aşağıdaki hareketler gerçekleşir.

![assets/rabbitmq_09.png](assets/rabbitmq_09.png)

Yani InsuranceService kendi tarafındaki ilgili Contract verisinin Quantity değerini günceller. Nitekim EmployeeService, bir çalışanını ilgili poliçe'den belli miktarda sattığına/aldığına dair bir olay bildirimi yapmıştır. EmployeeService tarafında ise aşağıdaki gibi bir çalışma zamanı gerçekleşir.

![assets/rabbitmq_10.png](assets/rabbitmq_10.png)

Buna göre InsuranceService'te kendi tarafını güncelledikten sonra tekrar bu sözleşme bilgisine istinaden bir güncelleme olduğunu duyurur. Bu olay EmployeeService tarafından dinlendiği için EmployeeService kendi tarafındaki Contract tablosunda güncellemeye gider.

## Web App Tarafı için Proxy Sınıflarının Üretilmesi

Eğitimde önyüz tarafındaki uygulamalar birer MVC projesi şeklinde oluşturulmakta. MVC projelerindeki Controller sınıflarının otomatik üretimi için Swagger şemasından yararlanabilen NSwag isimli CLI aracı kullanılmakta. Öncelikle bu aracın sistem yüklenmesi gerekiyor.

```bash
dotnet tool install --global NSwag.ConsoleCore

# sonrasında InsuranceService çalıştırılıp swagger dokümanı json formatında çekilmelidir
# Bu çalışma için örneğin http://localhost:5011/swagger/v1/swagger.json adresindeki içerik alınıp
# Insurance.WebApp projesindeki ApiProxy klasörüne kaydedilir.
# Sonrasında aşağıdaki komutlarla proxy sınıfının üretilmesi sağlanır. (Komut ApiProxy altında işletilmiştir)
nswag openapi2csclient /input:swagger.json /classname:InsuranceApiClient /namespace:Insurance.WebApp /output:InsuranceApiClient.cs
```

![assets/rabbitmq_11.png](assets/rabbitmq_11.png)

Burada yapılanları bir nevi Add Web Reference veya Add Service Reference işlemlerine benzetebiliriz. Insurance.WebApp projesindeki ApiProxy klasöründe otomatik olarak üretilen proxy sınıfı haricinde bunun kullanımını kolaylaştırmak üzere soyutlayan ve DI mekanizmasına kolayca monte edilmesini sağlayan yardımcı bir sınıfta vardır. IInsurancaApiHandler arayüzünü implemente eden InsuranceApiHandler sınıfı.

**EĞİTİMİM DEVAM EDİYOR. KONULARI İŞLEDİKÇE EKLEYECEĞİM.**
