using FundooNotes.Business.Services;
using FundooNotes.Models.Entity;
using FundooNotes.Repository.Context;
using FundooNotes.Repository.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FundooNotes.Repository.Services
{
    public class NoteRepository : INoteRepository
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public NoteRepository(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // Cache key generators
        private string GetNotesCacheKey(int userId) => $"notes:user:{userId}";
        private string GetNoteCacheKey(int noteId) => $"note:{noteId}";
        private string GetArchivedCacheKey(int userId) => $"archived:user:{userId}";
        private string GetTrashedCacheKey(int userId) => $"trashed:user:{userId}";

        // Clear user's cache
        private void ClearUserCaches(int userId)
        {
            _cache.Remove(GetNotesCacheKey(userId));
            _cache.Remove(GetArchivedCacheKey(userId));
            _cache.Remove(GetTrashedCacheKey(userId));
        }



        // Create Note Method...
        public Note CreateNote(Note note)
        {
            try
            {
                _context.Notes.Add(note);
                _context.SaveChanges();

                // clear cache
                ClearUserCaches(note.UserId);
                return note;
            }
            catch(Exception ex)
            {
                throw new Exception("Error occured while creating note: ", ex);
            }
        }

        public List<Note> GetNotesByUserId(int userId) 
        {
            try
            {
                string cacheKey = GetNotesCacheKey(userId);
                var cacheData = _cache.GetString(cacheKey);

                if(cacheData != null)
                {
                    return JsonSerializer.Deserialize<List<Note>>(cacheData);
                }
                // if cache miss then get from DB
                var notes = _context.Notes.Where(n => n.UserId == userId).ToList();

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                _cache.SetString(cacheKey, JsonSerializer.Serialize(notes), options);

                return notes;
            }
            catch(Exception ex)
            {
                throw new Exception("Error Occured while Fetching Notes ..: ", ex);
            }
           
        }

        public Note GetNoteById(int id)
        {
            try
            {

                string cacheKey = GetNoteCacheKey(id);
                var cacheData = _cache.GetString(cacheKey);
                if(cacheData != null)
                {
                    return JsonSerializer.Deserialize<Note>(cacheData);
                }


                var note =  _context.Notes.FirstOrDefault(n => n.id == id);

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                _cache.SetString(cacheKey, JsonSerializer.Serialize(note), options);

                return note;
            }
            catch(Exception ex)
            {
                throw new Exception("Error Occured while Fetching Note ..: ", ex);
            }
        }


        public Note UpdateNote(Note note)
        {
            try
            {
                note.UpdatedAt = DateTime.UtcNow;
                _context.Notes.Update(note);
                _context.SaveChanges();


                _cache.Remove(GetNoteCacheKey(note.id));
                ClearUserCaches(note.UserId);
                return note;
            }
            catch(Exception ex)
            {
                throw new Exception("Error Occured while Updating Note : ", ex);
            }
        }

        public bool DeleteNote(int id)
        {
            try
            {
                var note = _context.Notes.Find(id);
                if(note == null)
                {
                    return false;
                }
                _context.Notes.Remove(note);
                _context.SaveChanges();

                _cache.Remove(GetNoteCacheKey(id));
                ClearUserCaches(note.UserId);

                return true;
            }
            catch(Exception ex)
            {
                throw new Exception("Error Occured while deleting Note : ", ex);
            }
        }

        public Note ArchiveNote(int id)
        {
            try
            {
                var note = _context.Notes.Find(id);
                if(note == null)
                {
                    return null;
                }

                note.IsArchive = true;
                note.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();

                _cache.Remove(GetNoteCacheKey(id));
                ClearUserCaches(note.UserId);
                return note;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occurred while archiving note", ex);
            }
        }

        public Note UnarchiveNote(int id)
        {
            try
            {
                var note = _context.Notes.Find(id);
                if (note == null)
                {
                    return null;
                }

                note.IsArchive = false;
                note.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();

                _cache.Remove(GetNoteCacheKey(id));
                ClearUserCaches(note.UserId);
                return note;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occurred while unarchiving note", ex);
            }
        }

        public Note TrashNote(int id)
        {
            try
            {
                var note = _context.Notes.Find(id);
                if (note == null)
                {
                    return null;
                }

                note.IsTrash = true;
                note.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();

                _cache.Remove(GetNoteCacheKey(id));
                ClearUserCaches(note.UserId);
                return note;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occurred while trashing the note", ex);
            }
        }

        public Note RestoreNote(int id)
        {
            try
            {
                var note = _context.Notes.Find(id);
                if (note == null)
                {
                    return null;
                }

                note.IsTrash = false;
                note.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();

                _cache.Remove(GetNoteCacheKey(id));
                ClearUserCaches(note.UserId);
                return note;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occurred while restoring the note", ex);
            }
        }

        public List<Note> GetArchivedNotesByUserId(int userId)
        {
            try
            {
                string cacheKey = GetArchivedCacheKey(userId);
                var cachedData = _cache.GetString(cacheKey);
                if (cachedData != null)
                {
                    return JsonSerializer.Deserialize<List<Note>>(cachedData);
                }

                var notes= _context.Notes.Where(n => n.UserId==userId && n.IsArchive == true).ToList();

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };
                _cache.SetString(cacheKey, JsonSerializer.Serialize(notes), options);

                return notes;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occurred while fetching the archives notes : ", ex);
            }
        }


        public List<Note> GetTrashedNotesByUserId(int userId)
        {
            try
            {
                string cacheKey = GetTrashedCacheKey(userId);

                var cachedData = _cache.GetString(cacheKey);
                if (cachedData != null)
                {
                    return JsonSerializer.Deserialize<List<Note>>(cachedData);
                }
                var notes =  _context.Notes.Where(n => n.UserId == userId && n.IsTrash == true).ToList();

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                };
                _cache.SetString(cacheKey, JsonSerializer.Serialize(notes), options);

                return notes;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Occurred while fetching the trash notes : ", ex);
            }
        }


    }
}
