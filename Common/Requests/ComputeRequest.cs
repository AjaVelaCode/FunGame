using System.ComponentModel.DataAnnotations;
using FunGame.Common.Constants;

namespace FunGame.Common.Requests;

public class ComputeRequest
{
    [Required(ErrorMessage = "PlayerChoice is required.")]
    public GameChoice PlayerChoice { get; set; }

    [Required(ErrorMessage = "ComputerChoice is required.")]
    public GameChoice ComputerChoice { get; set; }
}