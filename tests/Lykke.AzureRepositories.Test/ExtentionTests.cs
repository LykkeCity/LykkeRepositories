using System;
using System.Collections.Generic;
using Lykke.Core;
using Xunit;

namespace Lykke.AzureRepositories.Test
{
    public class ExtentionTests
    {
        private readonly string _key = "test";
        private readonly string _encriptedString = "hfJd1cwIdDe5w8T2pRV1yA==";
        private readonly List<int> _testData = new List<int> {
            5,6,7,8,10
        };

        [Fact]
        public void EncriptionTest()
        {
            var s = _testData.Encrypt(_key);
            Assert.Equal(Convert.ToBase64String(s), _encriptedString);
        }


        [Fact]
        public void DecriptionTest()
        {
            var s = Convert.FromBase64String(_encriptedString).Decrypt<List<int>>(_key);
            Assert.Equal(5, s.Count);
        }
    }
}
