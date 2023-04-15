FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY /Aplicacao/*.sln ./Aplicacao/
COPY /Aplicacao/*.csproj ./Aplicacao/
COPY /Clientes/*.csproj ./Clientes/
COPY /Core/*.csproj ./Core/
COPY /Produtos/*.csproj ./Produtos/
COPY /Vendas/*.csproj ./Vendas/

#
RUN dotnet restore "./Aplicacao/AplicacaoGerenciamentoLoja.csproj"
RUN dotnet restore "./Clientes/Clientes.csproj"
RUN dotnet restore "./Core/Core.csproj"
RUN dotnet restore "./Produtos/Produtos.csproj"
RUN dotnet restore "./Vendas/Vendas.csproj"

#
# copy everything else and build app
COPY /Aplicacao/. ./Aplicacao/
COPY /Clientes/. ./Clientes/
COPY /Core/. ./Core/
COPY /Produtos/. ./Produtos/
COPY /Vendas/. ./Vendas/

#
WORKDIR /app/Aplicacao
RUN dotnet publish -c Release -o out 
WORKDIR /app/Clientes/
RUN dotnet publish -c Release -o out 
WORKDIR /app/Core/
RUN dotnet publish -c Release -o out
WORKDIR /app/Produtos/
RUN dotnet publish -c Release -o out 
WORKDIR /app/Vendas/
RUN dotnet publish -c Release -o out 

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/Aplicacao/out .
ENTRYPOINT ["dotnet", "AplicacaoGerenciamentoLoja.dll"]