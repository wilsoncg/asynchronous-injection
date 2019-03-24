# Asynchronous Injection

Sample code supporting the blog article [Asynchronous Injection](http://blog.ploeh.dk/2019/02/11/asynchronous-injection).

#### Todo

How about deposit taken after booking accepted:
* Booking service accepted but payment not taken
* Database unresponsive after booking
* Payment provider not responsive or rejects payment request

How about business rules where:
* High net worth clients can book the whole restaurant
* Undesirable customers cannot book
* Some customers are charged a flat booking fee, for some promotional customers the fee is waived, other customers are charged a percentage of their bill