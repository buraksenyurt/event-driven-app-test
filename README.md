# Event-Driven Yaklaşımlı .Net Çözümlerinde Test Edilebilirlik

Bu repoya konu olan çalışmadaki amaç .Net ile yazılmış ve event-driven yaklaşımını benimsemiş çözümlerde uçtan uca karmaşık testlerin nasıl yapılacağını anlamaya çalışmaktır. Microservice yaklaşımını benimseyen dağıtık sistem çözümlerinde test edilebilirlik önemlidir. Sistemin parçalarının her an test edilebilir olması code coverage değerlerini yukarı çekip olası üretim hatalarının önüne geçmek için elzemdir. Repodaki çalışmada amaç öncelikle Event-Driven yaklaşıma uygun basit bir çözüm geliştirmek sonrasında test konusunu ele almaktır.

## Senaryolar

Çözüm içerisinde iki servis yer alıyor. EmployeeService ve InsuranceService. Her ikisi de basit birer REST servisi ve kendi SQLite veritabanlarına sahipler. InsuranceService içerisinde yer alan controller sınıfı, kontrat oluşturmak, güncellemek ve listelemek için üç fonksiyon içeriyor. EmployeeService içerisinde yer alan controller'da çalışan oluşturmak, çalışan ve kontratları listelemek için üç fonksiyon içermekte. Yeni bir kontrat oluşturulması veya güncellenmesi EmployeeService için bir event anlamı taşımakta ve bu değişiklikler EmployeeService tarafından da değerlendirilmekte. 

Benzer durum sisteme yeni bir sigortacı(çalışan) eklendiği haller için de geçerli. Bir çalışan eklenmesi durumunda InsuranceService bundan haberdar olabilmeli. Servislerde gerçekleşen bazı eylemler sistem için bir olay(event) anlamı taşımakta. Bu noktada bir olayın muhatabının bu olayla ilgilenmesi için bir sisteme ihtiyaç var. RabbitMQ burada devreye giriyor. Servisler olaylarını asenkron olarak RabittMQ kuyruğuna bırakabiliyorlar. Olayın muhatapları bu aktiviteleri dinleyip kendi tarafları için gerekli eylemleri yapabiliyorlar. Bir servis tarafından üretilen bir nesnenin diğer servis tarafından da anlaşılabilmesi için ideal bir yapı. Dağıtık sistemlerde çok kullanılan bir çözüm.

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
