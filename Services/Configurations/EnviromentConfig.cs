using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDrones.Configurations;

public class EnvironmentConfig
{
    readonly IConfiguration _configuration;
    readonly string AmbienteKey;
    readonly string Ambiente;

    public EnvironmentConfig(IConfiguration configuration)
    {
        _configuration = configuration;
        AmbienteKey = _configuration.GetValue<string>("Ambiente");
        Ambiente = "Development";

        Ambiente = AmbienteKey switch
        {
            "sbdev" => "sbdev",
            "sbprd" => "sbprd",
            "prd" => "Produccion",
            _ => Ambiente
        };
    }

    public string GetAmbienteKey()
    {
        return AmbienteKey;
    }

    public string GetMercurioApiKey()
    {
        return _configuration.GetValue<string>($"Ambientes:{Ambiente}:MercurioServices:ApiKey");
    }
}
