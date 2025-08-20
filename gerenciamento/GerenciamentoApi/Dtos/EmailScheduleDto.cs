using System.ComponentModel.DataAnnotations;

namespace GerenciamentoApi.Dtos;

public record EmailScheduleDto(
    [Required(ErrorMessage = "to é obrigatório")] string To,
    [Required(ErrorMessage = "subject é obrigatório")] string Subject,
    string? Body,
    [Required(ErrorMessage = "send_time é obrigatório")] DateTime SendTime
);