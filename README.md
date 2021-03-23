# Test assignment considerations

## Assumptions

* Once unit is booked it can not be changed for this booking (in case of rental changes).
* If unit can not be changed during the booking period, it means that attempt to book rental that hasn't one consistent unit available throughout whole period fails.
* If more then one unit is available to book, the one with the lowest number is chosen

## Introduced complexity

* Noda time package provides better representation for dates without time component (booking system case) and for periods of time.
* FluentValidation and infrastructure around it guards business domain from invalid data and allows me to omit all this guards inside of domain code.
* Command/Queries are easier to map from API models and to validate and they allow to maintain binary backward-compatibility.
* DomainError and DomainException are compromise between hard to implement in C# [railway programming](https://davidelettieri.it/rop/%27tagged/union%27/%27railway/oriented/programming%27/c%23/2020/04/04/railway-oriented-programming-with-c.html) and exception-based error-handling. All domain errors have it's own representation which can be altered and handled differently. ApplicationException is much less typesafe and harder to handle.
* Setting entity identifiers through reflection is done to prevent public setter from exposing. It is similar to how Entity Framework sets generated ids and I prefer this approach if identification depends on storage state.

## Known issues
Since I was not asked to make production-ready application and assignment meant to be assessed by its maintainability and readability, I focused primarly on separation of concerns. To be run in production this solution lacks some features:
* Persistence. I did my best to make database-agnostic interfaces for entity persistence, however this efforts are rarely valuable because of the framework nature of most popular ORM solutions. Solution will require some adjustments to work with database and it heavily depends on chosen implementation.
* Atomicity. No attempts to implement transaction were made since most of the time it relies on particular database optimistic or pessimistic locking mechanisms.
* Concurrency. In memory repository with underlying dictionary is not thread-safe. It was possible to achieve some sort of thread-safety in my solution, however, due to sequencial entities identifiers generation it seems to be a big task, so I decided not to address this issue, since it was present in original code.
