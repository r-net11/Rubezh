using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GkWeb.Models.Plan;
using Microsoft.AspNetCore.Mvc;
using RubezhAPI.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GkWeb.Controllers
{
    [Route("api/[controller]")]
    public class PlansController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<PlanInfo> Get()
        {
			if (RubezhClient.ClientManager.PlansConfiguration == null
				|| RubezhClient.ClientManager.PlansConfiguration.Plans == null) {
				return null;
			}
			return RubezhClient.ClientManager.PlansConfiguration.AllPlans.Select(GetPlanInfo).ToList();
		}

		/// <summary>
		///     Рекурсивно получить основную информацию о плане и вложенных планах.
		/// </summary>
		/// <param name="plan">Объект плана.</param>
		/// <returns>Основная информация о плане, включая вложенные планы.</returns>
		private PlanInfo GetPlanInfo(Plan plan) {
			return new PlanInfo {
				Name = plan.Caption,
				Uid = plan.UID,
				Description = plan.Description,
				Width = plan.Width,
				Height = plan.Height,
				NestedPlans = plan.Children != null ? plan.Children.Select(GetPlanInfo) : null,
				ParentUid = plan.Parent != null ? plan.Parent.UID : (Guid?)null,
				IsFolder = plan is PlanFolder
			};
		}

		// GET api/values/5
		[HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
