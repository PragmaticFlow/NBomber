## AutoCluster Example

In this example we will run 2 NBomber instances (nodes) in the cluster mode.

- http_scenario will be executed on Coordinator
- http_scenario will be executed on Agent

You can take a look at `autocluster-config.json` config to check the ClusterSettings.

In order to run that demo you should spin-up NATS message broker via docker-compose CLI (there is `docker-compose.yaml`)

```
docker compose up
```

After this you need start 2 instances of NBomber.

Useful links:
- [Cluster overview](https://nbomber.com/docs/cluster/overview)
- [How to run cluster](https://nbomber.com/docs/cluster/run-cluster)
- [AutoCluster](https://nbomber.com/docs/cluster/auto-cluster)
