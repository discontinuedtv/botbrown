namespace BotBrown.Workers.Webserver.Api
{
    using BotBrown.Configuration;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Results;

    public class ConfigurationStatusController : ApiController
    {
        private IConfigurationManager configurationManager;

        public ConfigurationStatusController(IConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        [HttpGet]
        public JsonResult<ConfigurationsStatusViewModel> Get()
        {
            ConfigurationStatusViewModel[] status = configurationManager.CheckConfigurationStatus().Select(x => new ConfigurationStatusViewModel(x)).ToArray();
            var statusModels = new ConfigurationsStatusViewModel { Configurations = status };

            return Json(statusModels);
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values 
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5 
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5 
        public void Delete(int id)
        {
        }
    }
}
