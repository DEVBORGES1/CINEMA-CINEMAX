# CineMax - Sistema de Gestao de Cinema e Venda de Ingressos

Este repositorio contem o codigo-fonte do CineMax, uma aplicacao web desenvolvida na plataforma ASP.NET Core MVC para automacao de processos de cinemas, incluindo catalogacao de acervo, agendamento de sessoes, mapa de assentos interativo, fluxo de compras (checkout) e emissao de ingressos digitais com QR Code.

O projeto destaca-se pela portabilidade de dados (com migracao completa para SQLite) e uma interface responsiva premium com foco na experiencia do usuario.

---

## 1. Tecnologias Utilizadas

*   **Plataforma de Desenvolvimento:** .NET 9.0 (ASP.NET Core MVC)
*   **Mapeamento Objeto-Relacional (ORM):** Entity Framework Core 9.0
*   **Banco de Dados:** SQLite (arquivo local Cinema.db), garantindo que dados de filmes, sessoes e cadastros sejam portaveis ao mover a pasta do projeto
*   **Interface Front-End:** HTML5, CSS3 Customizado, JavaScript e Bootstrap
*   **Seguranca:** Autenticacao por Cookies integrada do ASP.NET Core com criptografia BCrypt para senhas de usuarios
*   **Geracao de Codigos:** Biblioteca integrada para geracao dinamica de QR Code de identificacao do ingresso

---

## 2. Arquitetura e Estrutura do Banco de Dados

A modelagem de dados foi estruturada para suportar as regras de negocio de um complexo de cinema de forma normalizada. As entidades principais e suas relacoes sao descritas a seguir:

*   **Pessoas (Person):** Mapeia todos os individuos do sistema. Suporta multiplas funcoes por meio de flags (Cliente, Estudante, Ator e Diretor).
*   **Perfis (Role):** Define papeis de acesso para seguranca de rotas (Administrador, Vendedor e Cliente).
*   **Filmes (Movie):** Armazena titulos, sinopses, duracao, link de poster oficial e associacao com generos e classificacao indicativa.
*   **Classificacao Indicativa (AgeRating):** Limites de faixa etaria obrigatorios para conformidade legal.
*   **Generos (Genre):** Temas de catalogacao dos filmes.
*   **Salas (Room):** Especifica a capacidade maxima e as dimensoes fisicas (linhas e colunas de assentos) de cada sala de exibicao.
*   **Sessoes (Session):** Conecta um filme a uma sala em uma data e horario especificos, definindo o valor unitario base do ingresso.
*   **Ingressos (Ticket):** Registra a compra individual para um assento especifico (linha e numero). Guarda a data de compra e o valor pago (com desconto automatico de 50% para estudantes cadastrados).
*   **Pedidos (Order):** Agrupador financeiro que consolida a transacao de ingressos e produtos adquiridos.
*   **Snacks (Snack):** Alimentos e bebidas disponiveis na bomboniere do cinema.
*   **Itens de Pedidos (SnackOrder):** Tabela de associacao com a quantidade e o historico do preco de aquisicao de cada snack.

### Relacionamentos de Destaque
*   Muitos-para-Muitos entre Person e Movie (para catalogar atores e diretores de um filme).
*   Muitos-para-Muitos entre Movie e Genre (um filme pode pertencer a varios generos).
*   Muitos-para-Um de Ticket para Session (uma sessao vende muitos ingressos ate a capacidade da sala).
*   Um-para-Muitos de Order para Ticket e SnackOrder (um pagamento consolidado contendo multiplos ingressos e lanches).

---

## 3. Funcionalidades Principais

### Autenticacao e Controle de Acesso
*   Login obrigatorio para conclusao do fluxo de checkout.
*   Autorizacao de rotas administrativas para criacao, edicao e exclusao de sessoes, filmes, salas e usuarios.

### Carrossel Inteligente e Posters Integrados
*   Banner inicial do site com efeito blur e escurecimento do plano de fundo para destacar o poster principal do filme.
*   Uso de proporcao padrao de cinema (Aspect Ratio 2:3) responsivo nos cards de filmes e detalhes, impedindo que rostos de atores ou titulos fiquem cortados em resolucoes Full HD, tablet ou mobile.
*   Imagens de posters oficiais de alta definicao salvas localmente no diretorio do servidor.

### Fluxo de Compra Dinamico (Checkout)
*   Passo 1: Selecao de quantidade de ingressos (Inteira ou Meia-Entrada).
*   Passo 2: Mapa de assentos interativo que carrega a quantidade exata de fileiras e colunas configuradas na Sala, bloqueando assentos ja reservados para aquela sessao.
*   Passo 3: Carrinho opcional de produtos de bomboniere.
*   Passo 4: Tela de resumo financeiro do pedido com insercao de dados de pagamento ficticios.

### Canhoto e Ingresso Digital
*   Apos a aprovacao da compra, a aplicacao renderiza o ingresso digital simulando um ticket fisico com canhoto destacavel e linha de picote pontilhada.
*   Cada ingresso exibe um QR Code gerado dinamicamente para validacao fisica na portaria.
*   Folha de estilo de impressao embutida que oculta menus, banners e botoes ao acionar a impressao do navegador, formatando e imprimindo individualmente os ingressos (um por pagina).

---

## 4. Como Executar o Projeto

### Pre-requisitos
*   SDK do .NET 9.0 instalado no computador.
*   Ferramentas globais do Entity Framework Core instaladas (opcional, para manipulacao direta de migrations):
    ```powershell
    dotnet tool install --global dotnet-ef
    ```

### Instrucoes de Execucao

1.  Abra o terminal na pasta raiz do projeto.
2.  Restaure as dependencias do NuGet:
    ```powershell
    dotnet restore
    ```
3.  Compile a aplicacao para certificar-se de que nao existem avisos ou erros:
    ```powershell
    dotnet build
    ```
4.  Execute a aplicacao (o banco de dados SQLite sera criado automaticamente na primeira execucao sob o arquivo `Cinema.db` e populado por meio do semeador):
    ```powershell
    dotnet run --project Cinema
    ```
5.  Acesse o sistema no navegador por meio da URL gerada no terminal (geralmente `http://localhost:5100` ou `https://localhost:7077`).

---

## 5. Credenciais de Teste (Semeador Automatico)

O sistema vem pre-populado com os seguintes perfis para facilitar os testes:

### Perfil Administrador
*   **E-mail:** admin@cinemax.com
*   **Senha:** 123456

### Perfil Vendedor
*   **E-mail:** vendedor@cinemax.com
*   **Senha:** 123456

### Perfil Cliente Comum
*   **E-mail:** cliente@cinemax.com
*   **Senha:** 123456

*   
