using FundooNotes.Business.Interface;
using FundooNotes.Models.DTOs.NoteDTOs;
using FundooNotes.Models.Entity;
using FundooNotes.Repository.Interfaces;

namespace FundooNotes.Business.Services
{
    public class NoteBL : INoteBL
    {
        private readonly INoteRepository _noteRepository;

        public NoteBL(INoteRepository noteRepo)
        {
            _noteRepository = noteRepo;
        }

        // Convert Notes into NoteResponseDTOs
        private NoteResponseDTO Mapping(Note note)
        {
            if (note == null)
            {
                return null;
            }
            return new NoteResponseDTO
            {
                Id = note.id,
                Title = note.Title,
                Description = note.Description,
                Color = note.Color,
                Image = note.Image,
                IsArchive = note.IsArchive,
                IsTrash = note.IsTrash,
                IsPinned=note.IsPinned,
                Reminder = note.Reminder,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                UserId = note.UserId
            };
        }

        // Convert List of Notes to List of NoteResponseDTOs
        private List<NoteResponseDTO> MapToDTOList(List<Note> notes)
        {
            return notes.Select(n => Mapping(n)).ToList();
        }



        public NoteResponseDTO CreateNote(CreateNoteDTO createNote, int userId)
        {
            try
            {
                if (createNote == null)
                {
                    throw new ArgumentNullException(nameof(createNote));
                }

                // DTO to entity conversion 
                var note = new Note
                {
                    Title = createNote.Title,
                    Description = createNote.Description,
                    Color = createNote.Color,
                    Image = createNote.Image,
                    Reminder = createNote.Reminder,
                    UserId = userId,  // JWT 
                    CreatedAt = DateTime.UtcNow,
                    IsArchive = false,
                    IsTrash = false
                };

                var createdNote = _noteRepository.CreateNote(note);

                return Mapping(createdNote);
            }

            catch (Exception ex)
            {
                throw new Exception($"Error creating note: {ex.Message}", ex);
            }
        }


        public List<NoteResponseDTO> GetAllNotes(int userId)
        {
            try
            {
                var allNotes = _noteRepository.GetNotesByUserId(userId).ToList();

                var activeNotes = allNotes.Where(n => !n.IsArchive && !n.IsTrash).ToList();
                return MapToDTOList(activeNotes);
            }
            catch(Exception ex)
            {
                throw new Exception($"Error Fetching notes : {ex.Message}", ex);
            }
        }


        public NoteResponseDTO GetNoteById(int noteId, int userId)
        {
            try
            {    
                var note = _noteRepository.GetNoteById(noteId);

                if(note == null)
                {
                    throw new Exception("Note Not found !");
                }

                if(note.UserId != userId)
                {
                    throw new UnauthorizedAccessException("You don't have permission to access this note !!");
                }

                return Mapping(note);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching note: {ex.Message}", ex);
            }
        }


        public NoteResponseDTO UpdateNote(UpdateNoteDTO updateNoteDto, int userId)
        {
            try
            {
                if (updateNoteDto == null)
                    throw new ArgumentNullException(nameof(updateNoteDto));

                var existingNote = _noteRepository.GetNoteById(updateNoteDto.id);
                if (existingNote == null)
                    throw new Exception("Note not found");

                if (existingNote.UserId != userId)
                    throw new UnauthorizedAccessException("You don't have permission to update this note");

                if (existingNote.IsArchive)
                    throw new Exception("Cannot update archived note, unarchive first.");

                if (existingNote.IsTrash)
                    throw new Exception("Cannot update trashed note, restore first.");

                if (!string.IsNullOrWhiteSpace(updateNoteDto.Title))
                    existingNote.Title = updateNoteDto.Title;

                if (updateNoteDto.Description != null)
                    existingNote.Description = updateNoteDto.Description;

                if (updateNoteDto.Color != null)
                    existingNote.Color = updateNoteDto.Color;

                if (updateNoteDto.Image != null)
                    existingNote.Image = updateNoteDto.Image;

                if (updateNoteDto.Reminder != null)
                {
                    existingNote.Reminder = updateNoteDto.Reminder;
                }
                else
                {
                    existingNote.Reminder = null;
                }

                if (updateNoteDto.IsArchive.HasValue)
                    existingNote.IsArchive = updateNoteDto.IsArchive.Value;

                if (updateNoteDto.IsTrash.HasValue)
                    existingNote.IsTrash = updateNoteDto.IsTrash.Value;
                if (updateNoteDto.IsPinned.HasValue)
                    existingNote.IsPinned = updateNoteDto.IsPinned.Value;

                var updatedNote = _noteRepository.UpdateNote(existingNote);

                return Mapping(updatedNote);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating note: {ex.Message}", ex);
            }
        }

        public bool DeleteNote(int noteId, int userId)
        {
            try
            {
                var note = _noteRepository.GetNoteById(noteId);

                if (note == null)
                {
                    throw new Exception("Note Not found !");
                }

                if (note.UserId != userId)
                {
                    throw new UnauthorizedAccessException("You don't have permission to delete this Note.");
                }

                return _noteRepository.DeleteNote(noteId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting note: {ex.Message}", ex);
            }

        }


        public NoteResponseDTO ArchiveNote(int noteId, int userId)
        {
            try
            {
                var note = _noteRepository.GetNoteById(noteId);

                if (note == null)
                {
                    throw new Exception("Note Not found !");
                }

                if (note.UserId != userId)
                {
                    throw new UnauthorizedAccessException("You don't have permission to Archive this Note.");
                }

                if (note.IsTrash)
                {
                    throw new Exception("Already in Trash , Cannot Archive.");
                }
                var archiveNote = _noteRepository.ArchiveNote(noteId);

                return Mapping(archiveNote);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Archive note: {ex.Message}", ex);
            }

        }

        public NoteResponseDTO UnarchiveNote(int noteId, int userId)
        {
            try
            {
                var note = _noteRepository.GetNoteById(noteId);

                if (note == null)
                {
                    throw new Exception("Note Not found !");
                }

                if (note.UserId != userId)
                {
                    throw new UnauthorizedAccessException("You don't have permission to Unarchive this Note.");
                }

                var unarchiveNote = _noteRepository.UnarchiveNote(noteId);

                return Mapping(unarchiveNote);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error UnArchive note: {ex.Message}", ex);
            }
        }

        public NoteResponseDTO TrashNote(int noteId, int userId)
        {
            try
            {
                var note = _noteRepository.GetNoteById(noteId);

                if (note == null)
                {
                    throw new Exception("Note Not found !");
                }

                if (note.UserId != userId)
                {
                    throw new UnauthorizedAccessException("You don't have permission to Move to trash this Note.");
                }

                if (note.IsArchive)
                {
                    note.IsArchive = false;
                    _noteRepository.UpdateNote(note);
                }

                var trashNote = _noteRepository.TrashNote(noteId);

                return Mapping(trashNote);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while trashing note: {ex.Message}", ex);
            }
        }

        // Restore note from trash
        public NoteResponseDTO RestoreNote(int noteId, int userId)
        {
            try
            {
                var note = _noteRepository.GetNoteById(noteId);

                if (note == null)
                {
                    throw new Exception("Note Not found !");
                }

                if (note.UserId != userId)
                {
                    throw new UnauthorizedAccessException("You don't have permission to restore this Note.");
                }

                var restoreNote = _noteRepository.RestoreNote(noteId);

                return Mapping(restoreNote);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while restoring note: {ex.Message}", ex);
            }
        }


        public List<NoteResponseDTO> GetArchivedNotes(int userId)
        {
            try
            {
                var archivedNotes = _noteRepository.GetArchivedNotesByUserId(userId);
                return MapToDTOList(archivedNotes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching archived notes: {ex.Message}", ex);
            }
        }


        public List<NoteResponseDTO> GetTrashedNotes(int userId)
        {
            try
            {
                var trashedNotes = _noteRepository.GetTrashedNotesByUserId(userId);
                return MapToDTOList(trashedNotes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching trashed notes: {ex.Message}", ex);
            }
        }
    }
}
