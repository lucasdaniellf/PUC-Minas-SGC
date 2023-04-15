# Sistema De Gerenciamento Comercial - Backend

Sistema online para gerenciamento de lojas comerciais que pode ser dividido em três módulos: **Módulo de Clientes, Módulo de Produtos, Módulo de Vendas.** Cada um dos módulos foi desenvolvido tendo em mente as necessidades das áreas a quais se direcionavam.

## Índice
1. [Arquitetura do Sistema](#arquitetura-do-sistema)
2. [Módulos do Sistema](#m%C3%B3dulos-do-sistema)
   - [Módulo de Clientes](#m%C3%B3dulo-de-clientes)
   - [Módulo de Produtos](#m%C3%B3dulo-de-produtos)
   - [Módulo de Vendas](#m%C3%B3dulo-de-vendas)
3. [Ferramentas Utilizadas](#ferramentas-utilizadas-no-desenvolvimento)
   - [Lista de Dependências](#lista-de-depend%C3%AAncias-do-projeto)
   - [Executando em modo de Desenvolvimento](#executando-em-modo-de-desenvolvimento)
   - [Executando via Docker](#executando-via-docker)
4. [Problemas Conhecidos](#problemas-conhecidos)
 
## Arquitetura do Sistema
Em termos de aplicação, embora esta esteja condensada em um único projeto para facilitar o desenvolvimento, a arquitetura do sistema foi baseada na arquitetura de 
microserviços, de modo que os três principais módulos do sistema (citados na seção anterior) não possuem dependências entre si, se utilizando de mensageria
quando necessitam notificar sobre alterações que ocorreram em seus domínios.

Além destes, o sistema possui ainda dois outros projetos: O projeto **Aplicação**, composto pela API e Serviços que consomem as mensagens publicadas via mensageria,
e o projeto **Core**, composto por classes e interfaces implementadas pelos módulos de negócios (Cliente, Produto e Venda) do sistema. Este último é composto por:

1. Commands : Interfaces para implementação dos comandos (requisições dos usuários) e sua execução.
2. EventMessages: Interfaces e Classe utilizada na implementação de eventos (notificações de operação realizada) e sua execução.
3. Infrastructure: Interfaces e classes utilizadas na implementação da camada de repositório e dados (Unit Of Work e Database Context).
4. MessageBroker: Interfaces e implementação dos componentes utilizados no disparo e recebimento de mensagens (Publisher e Subscriber).

Os módulos de negócios estão divididos em camadas:

1. Camada de Domínio: Encapsula toda a lógica de negócios para o módulo. Em nosso sistema, é subdividido em:
   1. Model: Contém as entidades e objetos de valores necessários ao módulo. 
   2. Repository: Interface do repositório para as chamadas feitas ao banco de dados.
2. Camada de Aplicação:
   1. Commands: Implementação dos comandos que podem ser executados pelo domínio.
   2. Events: Implementação das mensagens de eventos que são enviados ou recebidos pelo domínio através de mensageria.
   3. Query: Encapsula os DTOs (Data Objects) que representam a entidade para a camada de visualização (API) e o serviço de consultas ao banco de dados.
3. Camada de Infraestrutura (Infrastructure): Encapsula as classes necessárias para implementação do padrão Unit Of Work/Repository e a implementação da classe
de acesso à base de dados.

O diagrama da arquitetura do sistema pode ser observado abaixo:

![Esquema Arquitetural Projeto drawio](https://user-images.githubusercontent.com/70923700/215354066-9b5c0c17-f2cf-4d7e-bf90-d3062c393625.png)

## Módulos do Sistema

![Arquitetura Modulo Sistema drawio](https://user-images.githubusercontent.com/70923700/215354867-9b2ec59a-1ec0-4f97-a65b-64311655ffd3.png)

### Módulo de Clientes
1. Consulta de Clientes
2. Cadastro de Clientes
3. Atualização de Cadastro de Clientes
4. Gerenciamento de Status (Ativo/Inativo)

As operações 2, 3 e 4 publicam mensagens em canais específicos notificando a alteração de registros.

### Módulo de Produtos
1. Consulta de Produtos
2. Cadastro de Produtos
3. Atualização de Cadastro de Produtos
4. Gerenciamento de Status (Ativo/Inativo)
5. Gerenciamento de Estoque

As operações 2, 3, 4 e 5 publicam mensagens em canais específicos notificando a alteração de registros.

### Módulo de Vendas
O módulo de vendas, como o próprio nome sugere, é o responsável pelo gerenciamento e execução das operações de venda. O processo consiste basicamente de:

1. Dar entrada em um registro de venda (informando o código do cliente ao qual a venda está sendo feita)
2. Adicionar produtos a uma venda
3. Editar dados de venda (desconto aplicado, forma de pagamento)
4. Processar a venda

A operação de venda, no entanto, depende dos produtos e clientes que são gerenciados pelos outros dois módulos. A fim de evitar o acoplamento entre estes,
o módulo de venda possui em seu banco de dados as informações de produtos e clientes que são espelhadas de seus respectivos módulos. Por exemplo, para uma ação de cadastro de cliente, o espelhamento dos dados na base de vendas segue o fluxo abaixo:

1. Cliente cadastrado no módulo de Clientes.
2. Evento de Notificação de Cliente Cadastrado é disparado.
3. Consumidor recebe a mensagem e a repassa ao EventHandler do módulo de Vendas (ClienteAtualizadoEvent).
4. O cliente é registrado/atualizado no banco de dados do módulo de vendas.

Dessa forma, nenhum comando pode ser executado no módulo de vendas para cadastrar/atualizar/ativar/inativar clientes ou produtos, visto que essa é uma responsabilidade de seus próprios módulos, e a sincronização desses dados é realizada via mensageria, como já fora mencionado. Esse modelo isola os módulos e, em um posterior desenvolvimento que venha a separar os módulos em sistemas que operam independentemente, o Módulo de Vendas poderá continuar operando mesmo que o sistema de Clientes e/ou Produtos esteja indisponível.

Outro ponto a ser ressaltado: para que a operação de venda aconteça com sucesso, o registro da venda deve passar por todas as validações que estão definidas como regras de negócio na entidade Venda. O fluxo do processamento de uma venda (confirmação por parte do usuário e aprovação por parte do sistema) pode ser observado abaixo:

1. Requisição de confirmação de venda (Validação de regras de negócio e alteração do status da venda para processando).
2. Disparo de notificação informando a requisição da confirmação de venda.
3. Baixa (Reserva) do estoque no módulo de produtos.
4. Em caso de baixa com sucesso, disparo de notificação de alteração do estoque e alteração do status da venda para "aguardando pagamento". Nesse ponto também é publicado a mensagem para o sistema responsável pelo gerenciamento do pagamento;
5. Atualização de estoque na tabela de produtos do módulo de venda.
6. Em caso de retorno do pagamento concluído com sucesso, atualização do status da venda para aprovado. Em caso de pagamento não efetuado, atualização de status da venda para reprovado e disparo de notificação para repor o estoque que fora reservado no passo 3.

![Fluxo Venda v2 drawio](https://user-images.githubusercontent.com/70923700/228712188-bd9f3d83-4ecb-4c10-b0ba-fd1afa37cfce.png)

## Ferramentas Utilizadas no Desenvolvimento
1. .NET 6
2. Docker e Redis ver. 7.0.8 (Mensageria)
3. Sqlite3 (Banco de Dados)

### Lista de Dependências do Projeto
1. Dapper
2. Microsoft.Data.Sqlite
3. Microsoft.Data.Sqlite.Core
4. Microsoft.Extensions.Options
5. Newtonsoft.Json
6. StackExchange.Redis
7. Swashbuckle.AspNetCore

### Executando em modo de Desenvolvimento
1. Puxar e rodar a imagem do Redis diretamente do Docker Hub executando via cmd:
```
docker run --name some-redis -p 6379:6379 -d redis
```
2. Clonar repositório localmente e rodá-lo utilizando VisualStudio 2022
```
git clone https://github.com/lucasdaniellf/SistemaDeGerenciamentoComercial
```
![image](https://user-images.githubusercontent.com/70923700/215342966-4fc0d0c0-2acb-4a79-b4a5-6c5d552fc2ae.png)

### Executando via docker
1. Clonar repositório localmente e rodá-lo utilizando VisualStudio 2022
```
git clone https://github.com/lucasdaniellf/SistemaDeGerenciamentoComercial
```
2. Na pasta raiz do projeto, onde está localizado o arquivo docker-compose.yaml, rodar o seguinte comando via cmd:

```
docker compose up
```
3. Acessar a API utilizando a interface do swagger em:
```
http://localhost:5006/swagger/index.html 
```

![image](https://user-images.githubusercontent.com/70923700/215369226-aaabedd7-ad96-41e4-8e52-d852de4baad4.png)














