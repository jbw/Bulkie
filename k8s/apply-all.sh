kubectl apply \
    -f namespaces.yaml \
    -f jaeger.yaml \
    -f bulkie.yaml \
    -f bulkie-file-processor.yaml \
    -f dapr/components/pubsub.yaml \
    -f dapr/components/statestore.yaml \
    -f dapr/configuration/config.yaml \