using FundooNotes.Models.Entity;

namespace FundooNotes.Repository.Interfaces
{
    public interface INoteRepository
    {
        Note CreateNote(Note note);
        List<Note> GetNotesByUserId(int userId);
        Note GetNoteById(int id);
        Note UpdateNote(Note note);
        bool DeleteNote(int id);
        Note ArchiveNote(int id);
        Note UnarchiveNote(int id);
        
        Note TrashNote(int id);
        Note RestoreNote(int id);
        List<Note> GetArchivedNotesByUserId(int userId);
        List<Note> GetTrashedNotesByUserId(int userId);
    }
}
