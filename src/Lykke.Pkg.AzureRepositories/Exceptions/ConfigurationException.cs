﻿namespace Lykke.AzureRepositories.Exceptions
{
    public class ConfigurationException : System.Exception
    {
        public ConfigurationException()
        {
        }

        public ConfigurationException(string message) :
            base(message)
        {
        }

        public ConfigurationException(string message, System.Exception inner) :
            base(message, inner)
        {
        }
    }
}