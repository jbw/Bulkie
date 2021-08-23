

## Minikube

```
minikube start --addons=ingress
```

```
eval $(minikube docker-env)
```

### Building local images

```
docker build -t bulkie/bulkie-api -f src/Services/Bulkie/Bulkie.API/Dockerfile .
docker build -t bulkie/bulkie-fileprocessor-api -f src/Services/Bulkie/BulkieFileProcessor.API/Dockerfile .
```

## Dapr

```
dapr init -k
```

## Helm
```
helm repo add bitnami https://charts.bitnami.com/bitnami
```

```
helm repo update
```

### RabbitMQ

```
helm install rabbitmq bitnami/rabbitmq -n bulkie --set auth.username=rabbitmq,auth.password=rabbitmq
```

### Redis
```
helm install redis bitnami/redis -n bulkie
```

### Postgres
```
helm install postgres bitnami/postgresql -n bulkie
```

### Minio
```
helm install minio bitnami/minio -n bulkie --set defaultBuckets=files
```

Create bucket called: `files`

### Jaeger 

```
helm repo add jaegertracing https://jaegertracing.github.io/helm-charts
```

```
helm install jaeger-operator jaegertracing/jaeger-operator -n observability
```

[Documentation](https://github.com/jaegertracing/helm-charts/tree/main/charts/jaeger-operator)

## Deploy application

```
cd k8s
./apply-all.sh
```