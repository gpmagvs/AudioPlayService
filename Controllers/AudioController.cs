using AudioPlayService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AudioPlayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioController : ControllerBase
    {
        private AudioPlayControlCenter bgService;

        public AudioController(AudioPlayControlCenter bgService)
        {
            this.bgService = bgService;
        }


        [HttpGet("Information")]
        public async Task<IActionResult> GetInformation()
        {
            AudioPlayerInformation info = bgService.GetInformation();
            return Ok(info);
        }

        [HttpPost("PlayAudio")]
        public async Task<IActionResult> PlayAudio(string audioFilePath)
        {
            bgService.PlayAudioStandalone(audioFilePath);
            return Ok();
        }


        [HttpPost("StopAudio")]
        public async Task<IActionResult> StopAudio(string audioFilePath)
        {
            bgService.StopAudio(audioFilePath);
            return Ok();
        }


        [HttpPost("StopAll")]
        public async Task<IActionResult> StopAll()
        {
            bgService.StopAll();
            return Ok();
        }

        [HttpPost("AddAudioToQueue")]
        public async Task<IActionResult> AddAudioToQueue(string audioFilePath)
        {
            (bool confirm, string message) = await bgService.AddAudioToPlayQueue(audioFilePath);
            Console.WriteLine(message);
            return Ok();
        }

        [HttpPost("RemoveAudioFromQueue")]
        public async Task<IActionResult> RemoveAudioFromQueue(string audioFilePath)
        {
            (bool confirm, string message) = await bgService.RemoveAudioFromPlayQueue(audioFilePath);
            Console.WriteLine(message);
            return Ok();
        }
    }
}
