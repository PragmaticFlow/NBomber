﻿open FSharpProd

[<EntryPoint>]
let main argv =

    //DataFeed.DataFeedTest.run()

    //HelloWorld.HelloWorldExample.run()
    //HelloWorld.CustomSettingsExample.run()

    HttpTests.SimpleHttpTest.run()
    //HttpTests.AdvancedHttpTest.run()
    //HttpTests.AdvancedHttpWithConfig.run()
    //HttpTests.TracingHttp.run()

    //Logging.ElasticSearchLogging.run()

    //RealtimeReporting.InfluxDbReporting.run()

    //MongoDb.MongoDbTest.run()

    //JsonConfig.JsonConfigExample.run()

    0 // return an integer exit code

