## Integração do Prometheus e Grafana

Nesta ponderada ASP.NET Core está instrumentado com **OpenTelemetry**, permitindo expor métricas customizadas e nativas para monitoramento com **Prometheus e Grafana**.

> *Nota: A integração foi feita **sem Docker**, pois minha máquina atualmente apresenta problemas com a execução de containers.*

### Tecnologias usadas

* [.NET 8 / ASP.NET Core](https://dotnet.microsoft.com/)
* [OpenTelemetry Metrics](https://opentelemetry.io/docs/instrumentation/net/)
* [Prometheus](https://prometheus.io/)
* [Grafana](https://grafana.com/)

## Como rodar

### 1. Clone o projeto e instale as dependências

```bash
dotnet restore
```

### 2. Execute o projeto

```bash
dotnet run
```

O app estará disponível em:
[`http://localhost:5000`](http://localhost:5000)

As métricas estarão em:
[`http://localhost:5000/metrics`](http://localhost:5000/metrics)

### 3. Baixe e execute o Prometheus

1. Baixe o Prometheus: [https://prometheus.io/download/](https://prometheus.io/download/)
2. Extraia o zip e crie um arquivo `prometheus.yml` com o conteúdo:

```yaml
global:
  scrape_interval: 5s

scrape_configs:
  - job_name: 'aspnetcore_app'
    metrics_path: /metrics
    static_configs:
      - targets: ['localhost:5000']
```

3. Rode o Prometheus:

```bash
prometheus.exe --config.file=prometheus.yml
```

Acesse: [http://localhost:9090](http://localhost:9090)

### 4. Baixe e execute o Grafana

1. Baixe o Grafana: [https://grafana.com/grafana/download](https://grafana.com/grafana/download/)

2. Execute `grafana-server.exe` (ou use o instalador `.msi`)

3. Acesse: [http://localhost:3000](http://localhost:3000)
   Login padrão: `admin / admin`

4. Vá em **Configuration → Data Sources → Add data source**

5. Escolha **Prometheus**

6. Defina a URL como: `http://localhost:9090`

7. Clique em **Save & Test**

## Métricas disponíveis

O projeto expõe métricas nativas via OpenTelemetry, que estão sendo monitoradas pelo Prometheus e visualizadas no Grafana. Abaixo, as principais métricas utilizadas no painel:

| Métrica                                        | Descrição                                                                                                         |
| ---------------------------------------------- | ----------------------------------------------------------------------------------------------------------------- |
| **`kestrel_active_connections`**               | Número de conexões HTTP ativas no servidor Kestrel. Reflete quantas conexões simultâneas estão abertas com o app. |
| **`http_server_active_requests`**              | Quantidade de requisições HTTP ativas no momento. Representa a carga instantânea no servidor.                     |
| **`http_server_request_duration_seconds_sum`** | Soma do tempo (em segundos) de todas as requisições HTTP servidas. Permite análise de performance acumulada.      |

![Painel do Grafana com métricas ASP.NET](/Images/image%20(14).png)

### Observações:

* As métricas foram configuradas para serem expostas em `/metrics`, com bucket de histogramas personalizados para análises mais granulares.
* O painel do Grafana inclui:

  * Gráfico de conexões ativas (`kestrel_active_connections`)
  * Indicador do total de requisições simultâneas (`http_server_active_requests`)
  * Gráfico de linha com a soma do tempo das requisições (`http_server_request_duration_seconds_sum`)