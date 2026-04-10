using FundooNotes.Models.DTOs.NoteDTOs;

namespace FundooNotes.Business.Interface
{
    public interface INoteBL
    {
        NoteResponseDTO CreateNote(CreateNoteDTO createNote, int id);
        
        // Get all notes (except trash and archieve)
        List<NoteResponseDTO> GetAllNotes(int userId);

        // Get a Single Note
        NoteResponseDTO GetNoteById(int noteId, int userId);

        // Update Note
        NoteResponseDTO UpdateNote(UpdateNoteDTO updateNoteDto, int userId);

        // Permanently delete a note
        bool DeleteNote(int noteId, int userId);

        // Archive a note
        NoteResponseDTO ArchiveNote(int noteId, int userId);

        // Unarchive a note
        NoteResponseDTO UnarchiveNote(int noteId, int userId);

        // Move note to trash
        NoteResponseDTO TrashNote(int noteId, int userId);

        // Restore note from trash
        NoteResponseDTO RestoreNote(int noteId, int userId);

        // Get all archived notes for current user
        List<NoteResponseDTO> GetArchivedNotes(int userId);

        // Get all trashed notes for current user
        List<NoteResponseDTO> GetTrashedNotes(int userId);
    }
}
