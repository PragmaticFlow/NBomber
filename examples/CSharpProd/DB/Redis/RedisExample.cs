﻿using NBomber.CSharp;

namespace CSharpProd.DB.Redis;

public class RedisExample
{
    public void Run()
    {
        NBomberRunner.RegisterScenarios(
            new RedisInitScenario().Create(),
            new RedisReadScenario().Create(),
            new RedisWriteScenario().Create()
        )
        .LoadConfig("./DB/Redis/config.json")
        .Run();
    }
}
