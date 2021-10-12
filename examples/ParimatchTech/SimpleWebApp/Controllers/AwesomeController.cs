using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpleWebApp.Helpers;

namespace SimpleWebApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AwesomeController : ControllerBase
    {
        private readonly KafkaProducer _kafkaProducer;
        private static ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();

        public AwesomeController(KafkaProducer kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }

        [HttpPost]
        public List<string> GenerateGuidIds([Required] int howManyIds)
        {
            var list = new List<string>();

            for (int i = 0; i < howManyIds; i++)
            {
                list.Add(Guid.NewGuid().ToString("N"));
            }

            return list;
        }

        [HttpPost]
        public List<string> GenerateGuidIdsAndSaveToCache([Required] int howManyIds)
        {
            var list = new List<string>();

            for (int i = 0; i < howManyIds; i++)
            {
                var guid = Guid.NewGuid().ToString("N");
                list.Add(guid);
                _cache.TryAdd(guid, guid);
            }

            return list;
        }

        [HttpPost]
        public async Task<List<string>> GenerateGuidIdsAndWriteToKafka([Required] int howManyIds)
        {
            var list = new List<string>();

            for (int i = 0; i < howManyIds; i++)
            {
                list.Add(Guid.NewGuid().ToString("N"));
            }

            await _kafkaProducer.ProduceMessage("GuidIds", JsonConvert.SerializeObject(list));

            return list;
        }
    }
}