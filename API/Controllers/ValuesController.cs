using Microsoft.AspNetCore.Mvc;
using Businesslogic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static List<Machine> machines = new List<Machine>
        {
            new Machine { MachineID = 1, CurrentState = "Running", SignalLightState = "Green" },
            new Machine { MachineID = 2, CurrentState = "Stopped", SignalLightState = "Red" }
        };

        // GET: api/machines
        [HttpGet]
        public ActionResult<IEnumerable<Machine>> GetMachines()
        {
            return machines;
        }

        // GET api/machines/5
        [HttpGet("{id}")]
        public ActionResult<Machine> GetMachine(int id)
        {
            var machine = machines.FirstOrDefault(m => m.MachineID == id);
            if (machine == null)
            {
                return NotFound();
            }
            return machine;
        }

        // POST api/machines
        [HttpPost]
        public ActionResult<Machine> PostMachine([FromBody] Machine machine)
        {
            machines.Add(machine);
            return CreatedAtAction(nameof(GetMachine), new { id = machine.MachineID }, machine);
        }

        // PUT api/machines/5
        [HttpPut("{id}")]
        public IActionResult PutMachine(int id, [FromBody] Machine machine)
        {
            var existingMachine = machines.FirstOrDefault(m => m.MachineID == id);
            if (existingMachine == null)
            {
                return NotFound();
            }
            existingMachine.CurrentState = machine.CurrentState;
            existingMachine.SignalLightState = machine.SignalLightState;
            return NoContent();
        }

        // DELETE api/machines/5
        [HttpDelete("{id}")]
        public IActionResult DeleteMachine(int id)
        {
            var machine = machines.FirstOrDefault(m => m.MachineID == id);
            if (machine == null)
            {
                return NotFound();
            }
            machines.Remove(machine);
            return NoContent();
        }
    }
}