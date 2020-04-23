FROM envoyproxy/envoy:v1.11.1

COPY ./envoy-ssl.crt /etc/envoy-ssl.crt
COPY ./envoy-ssl.key /etc/envoy-ssl.key

COPY ./envoy.yml /etc/envoy/envoy.yaml

