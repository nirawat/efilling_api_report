using THD.Core.Api.Business.Interface;
using THD.Core.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;
using System.Text;
using THD.Core.Api.Helpers;
using System.Threading;
using System.Collections.Generic;
using THD.Core.Api.Models.Config;
using System.IO;
using THD.Core.Api.Models.ReportModels;

namespace THD.Core.Api.Private.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrivateDocMenuReportController : ControllerBase
    {
        private readonly IDocMenuReportService _IDocMenuReportService;
        private IHttpContextAccessor _httpContextAccessor;
        private IEnvironmentConfig _EnvironmentConfig;

        public PrivateDocMenuReportController(
            IDocMenuReportService IDocMenuReportService,
            IHttpContextAccessor httpContextAccessor,
            IEnvironmentConfig EnvironmentConfig)
        {
            _IDocMenuReportService = IDocMenuReportService;
            _httpContextAccessor = httpContextAccessor;
            _EnvironmentConfig = EnvironmentConfig;
        }

        [HttpGet("GetReportR1_2/{DocId}")]
        public async Task<IActionResult> GetReportR1_2(int DocId)
        {
            model_rpt_1_file e = await _IDocMenuReportService.GetReportR1_2Async(DocId);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }

        [HttpGet("GetReportR3/{DocId}")]
        public async Task<IActionResult> GetReportR3(int DocId)
        {
            model_rpt_3_file e = await _IDocMenuReportService.GetReportR3Async(DocId);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }

        [HttpGet("GetReportR4/{DocId}")]
        public async Task<IActionResult> GetReportR4(int DocId)
        {
            model_rpt_4_file e = await _IDocMenuReportService.GetReportR4Async(DocId);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }

        [HttpGet("GetReportR5/{DocId}")]
        public async Task<IActionResult> GetReportR5(int DocId)
        {
            model_rpt_5_file e = await _IDocMenuReportService.GetReportR5Async(DocId);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }

        [HttpGet("GetReportR6/{DocId}")]
        public async Task<IActionResult> GetReportR6(int DocId)
        {
            model_rpt_6_file e = await _IDocMenuReportService.GetReportR6Async(DocId);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }

        [HttpGet("GetReportR7/{DocId}")]
        public async Task<IActionResult> GetReportR7(int DocId)
        {
            model_rpt_7_file e = await _IDocMenuReportService.GetReportR7Async(DocId);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }

        [HttpGet("GetReportR8/{DocId}/{Type}")]
        public async Task<IActionResult> GetReportR8(int DocId, string Type)
        {
            model_rpt_8_file e = await _IDocMenuReportService.GetReportR8Async(DocId, Type);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }

        [HttpGet("GetReportR9/{DocId}")]
        public async Task<IActionResult> GetReportR9(int DocId)
        {
            model_rpt_9_file e = await _IDocMenuReportService.GetReportR9Async(DocId);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }

        [HttpGet("GetReportR10/{DocId}")]
        public async Task<IActionResult> GetReportR10(int DocId)
        {
            model_rpt_10_file e = await _IDocMenuReportService.GetReportR10Async(DocId);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }

        [HttpGet("GetReportR11/{DocId}")]
        public async Task<IActionResult> GetReportR11(int DocId)
        {
            model_rpt_11_file e = await _IDocMenuReportService.GetReportR11Async(DocId);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }

        [HttpGet("GetReportR12/{DocId}/{Type}")]
        public async Task<IActionResult> GetReportR12(int DocId, int Type)
        {
            model_rpt_12_file e = await _IDocMenuReportService.GetReportR12Async(DocId, Type);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }

        [HttpGet("GetReportR13/{DocId}/{type}")]
        public async Task<IActionResult> GetReportR13(int DocId, int type)
        {
            model_rpt_13_file e = await _IDocMenuReportService.GetReportR13Async(DocId, type);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }

        [HttpGet("GetReportR14/{DocId}")]
        public async Task<IActionResult> GetReportR14(int DocId)
        {
            model_rpt_14_file e = await _IDocMenuReportService.GetReportR14Async(DocId);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }

        [HttpGet("GetReportR15/{DocId}")]
        public async Task<IActionResult> GetReportR15(int DocId)
        {
            model_rpt_15_file e = await _IDocMenuReportService.GetReportR15Async(DocId);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }

        [HttpGet("GetReportR17_18/{DocId}")]
        public async Task<IActionResult> GetReportR17_18(int DocId)
        {
            model_rpt_17_file e = await _IDocMenuReportService.GetReportR17_18Async(DocId);

            if (e != null) return Ok(e);
            else return BadRequest(e);
        }


    }
}