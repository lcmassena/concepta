# concepta

Solução teste para consultar dados de um ticket.

Esse modelo arquitetural é baseado em Event Sourcing, usando o Mediart como engine de Mediação de exeução de comandos.

A solução contém 5 projetos, sendo eles:
- Hexagonal.API 
  Uma API .net core, responsável por disparar os eventos
- Application
  Core do negócio. Onde deverão estar concentrados os objetos de negócio e regras.
- Repositories
  Criei dois repositories com nomes separados apenas para exemplificar a segregação de responsabilidade e a flexibilidade desse modelo arquitetural.
- Core
  Possui funcionalidades que podem/devem ser reutilizadas em outros projetos.
  
 
