using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FundooNotes.Business.Interface;
using System.Security.Claims;
using FundooNotes.Models.DTOs.NoteDTOs;
//using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

namespace FundooNotes.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

   
    public class NoteController : ControllerBase
    {
        private readonly INoteBL _noteBL;

        public NoteController(INoteBL noteBl)
        {
            _noteBL = noteBl;
        }


        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("Unauthorized Access !");
            }
            return int.Parse(userIdClaim);
        }

        [HttpGet("get-all-notes")]
        public IActionResult GetAllNotes()
        {
            try
            {
                int userId = GetUserId();
                var notes = _noteBL.GetAllNotes(userId);

                return Ok(new { success = true, data = notes });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }

        }


        [HttpGet("get-note-by-id/{id}")]
        public IActionResult GetNoteById(int id)
        {
            try
            {
                int userId = GetUserId();
                var note = _noteBL.GetNoteById(id, userId);

                return Ok(new { success = true, data = note });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        // Create Note 
        [HttpPost("create-note")]
        public IActionResult CreateNote([FromBody] CreateNoteDTO createNoteDTO)
        {
            try
            {
                int userId = GetUserId();
                var createNote = _noteBL.CreateNote(createNoteDTO, userId);
                return CreatedAtAction(nameof(GetNoteById), new { id = createNote.Id }, new { success = true, Data = createNote });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        // Update Note 
        [HttpPut("update-note/{id}")]
        public IActionResult UpdateNote([FromBody] UpdateNoteDTO updatenoteDTO, int id)
        {
            try
            {
                if (id != updatenoteDTO.id) 
                {
                    return BadRequest(new { success = false, message = "ID Mismatch" });
                }
                int userId = GetUserId();
                var updateNote = _noteBL.UpdateNote(updatenoteDTO, userId);
                return Ok(new { success = true, Data = updateNote });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }



        [HttpDelete("delete/{id}")]

        public IActionResult DeleteNote(int id)
        {
            try
            {
                int userId = GetUserId();
                var result = _noteBL.DeleteNote(id, userId);
                return Ok(new { success = result, message = "Note Deleted." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }

        }


        [HttpPut("{id}/archive")]
        public IActionResult ArchiveNote(int id)
        {
            try
            {
                int userId = GetUserId();
                var result = _noteBL.ArchiveNote(id, userId);
                return Ok(new { success = result, message = "Note Archived.", data=result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}/unarchive")]
        public IActionResult UnarchiveNote(int id)
        {
            try
            {
                int userId = GetUserId();
                var result = _noteBL.UnarchiveNote(id, userId);
                return Ok(new { success = result, message = "Note Unarchived.", data = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}/trash")]
        public IActionResult TrashNote(int id)
        {
            try
            {
                int userId = GetUserId();
                var trashedNote = _noteBL.TrashNote(id, userId);
                return Ok(new { success = true, data = trashedNote, message = "Note moved to trash" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpPut("{id}/restore")]
        public IActionResult RestoreNote(int id)
        {
            try
            {
                int userId = GetUserId();
                var restoredNote = _noteBL.RestoreNote(id, userId);
                return Ok(new { success = true, data = restoredNote, message = "Note restored from trash" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("archived")]
        public IActionResult GetArchivedNotes()
        {
            try
            {
                int userId = GetUserId();
                var notes = _noteBL.GetArchivedNotes(userId);
                return Ok(new { success = true, data = notes });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("trashed")]
        public IActionResult GetTrashedNotes()
        {
            try
            {
                int userId = GetUserId();
                var notes = _noteBL.GetTrashedNotes(userId);
                return Ok(new { success = true, data = notes });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

    }
}
