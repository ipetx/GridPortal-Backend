using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MainAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PowerflowController : ControllerBase
    {
        [HttpGet("Import")]
        public IActionResult Import([FromQuery] string fileName)
        {
            /**************************************** write to the database *******************************************/
            //var status = InsertHandler.Run("C:\\05_RnD\\Dashboard\\Examples\\23SSWG_2025_WIN1_U3_Final_10142024.rawx");
            //var status = InsertHandler.Run("C:\\05_RnD\\GridPortal\\Examples\\sample_nb.rawx");
            var status = InsertHandler.Run(fileName);

            return Ok(status);
        }

        [HttpGet("Export")]
        public IActionResult Export()
        {
            // require outDir just after the command
            var outDir = "C:\\05_RnD\\GridPortal\\Examples";

            var fileName = $"{(DateTime.Now).ToString("MM_dd_yyyy_HH_mm_ss")}";
            var pattern = $"case_{fileName}.rawx";

            ExportHandler.RunAll(outDir, pattern);

            return Ok("");
        }
    }
}
