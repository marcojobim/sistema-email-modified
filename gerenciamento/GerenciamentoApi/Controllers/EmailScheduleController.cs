using Gerenciamento.Shared.Data;
using GerenciamentoApi.Dtos;
using Gerenciamento.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GerenciamentoApi.Controllers;

[ApiController]
public class EmailScheduleController : ControllerBase
{
    private readonly ApiDbContext _context;

    public EmailScheduleController(ApiDbContext context)
    {
        _context = context;
    }

    [HttpPost("/submit")]
    public async Task<IActionResult> Submit([FromBody] EmailScheduleDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var emailSchedule = new EmailSchedule
        {
            To = dto.To,
            Subject = dto.Subject,
            Body = dto.Body,
            SendTime = dto.SendTime
        };

        try
        {
            _context.EmailSchedules.Add(emailSchedule);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Agendamento de email criado com sucesso!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao salvar os dados. Causa: {InnerExceptionMessage}", ex.InnerException?.Message });
        }
    }
}

