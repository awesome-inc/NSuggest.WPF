﻿using Newtonsoft.Json.Linq;

namespace TestSuggestions.ElasticSearch
{
    internal class TestData
    {
        #region sample hits

        internal static readonly JToken Hits = JToken.Parse(@"{
  ""took"": 22,
  ""timed_out"": false,
  ""_shards"": {
    ""total"": 5,
    ""successful"": 5,
    ""failed"": 0
  },
  ""hits"": {
    ""total"": 6,
    ""max_score"": 1,
    ""hits"": [
      {
        ""_index"": ""movies"",
        ""_type"": ""movie"",
        ""_id"": ""b3d0cb06-1173-445d-8b01-de53ab485169"",
        ""_score"": 1,
        ""fields"": {
          ""director"": [
            ""Tobias""
          ],
          ""producer"": [
            ""Thomate""
          ]
}
      },
      {
        ""_index"": ""movies"",
        ""_type"": ""movie"",
        ""_id"": ""9af4934e-7ab5-4621-8e28-db1bc80376c2"",
        ""_score"": 1,
        ""fields"": {
          ""director"": [
            ""Jane Doe""
          ],
          ""producer"": [
            ""Thomas""
          ]
        }
      },
      {
        ""_index"": ""movies"",
        ""_type"": ""movie"",
        ""_id"": ""dbe8ec91-5cf6-42dc-94fa-0a67f3618049"",
        ""_score"": 1,
        ""fields"": {
          ""director"": [
            ""Jan""
          ],
          ""producer"": [
            ""Thomas""
          ]
        }
      },
      {
        ""_index"": ""movies"",
        ""_type"": ""movie"",
        ""_id"": ""7d9e4d4e-dca1-49b7-ae35-9ac177e98a9f"",
        ""_score"": 1,
        ""fields"": {
          ""director"": [
            ""Horst""
          ],
          ""producer"": [
            ""Thomas""
          ]
        }
      },
      {
        ""_index"": ""movies"",
        ""_type"": ""movie"",
        ""_id"": ""f74683d5-532a-40f0-9e13-0a9e20d92cb2"",
        ""_score"": 1,
        ""fields"": {
          ""producer"": [
            ""Thomas Smith""
          ]
        }
      },
      {
        ""_index"": ""movies"",
        ""_type"": ""movie"",
        ""_id"": ""5484bedd-c79f-432d-a862-04e1c44bfdab"",
        ""_score"": 1,
        ""fields"": {
          ""director"": [
            ""Marcel""
          ],
          ""producer"": [
            ""Thomas""
          ]
        }
      }
    ]
  }
}");
        #endregion
    }
}