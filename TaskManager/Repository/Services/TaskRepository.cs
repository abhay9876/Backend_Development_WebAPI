using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TaskManager.Business.Service;
using TaskManager.Models.Entity;
using TaskManager.Repository.Context;
using TaskManager.Repository.Interface;
using static Azure.Core.HttpHeader;

namespace TaskManager.Repository.Services
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskManagerDbContext _context;
        private readonly IDistributedCache _cache;
        public TaskRepository(TaskManagerDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }



        private string GetNotesCacheKey(int userId) => $"tasks:user:{userId}";
        private string GetNoteCacheKey(int taskId) => $"task:{taskId}";


        private void ClearCache(int userId)
        {
            _cache.Remove(GetNoteCacheKey(userId));
            _cache.Remove(GetNotesCacheKey(userId));

        }

        public TaskM CreateTask(TaskM task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
            ClearCache(task.UserId);
            return task;
        }


        public TaskM GetTaskById(int id)
        {
            try
            {
                var cacheKey = GetNoteCacheKey(id);
                var cacheData = _cache.GetString(cacheKey);

                if(cacheData != null)
                {
                    return JsonSerializer.Deserialize<TaskM>(cacheData);
                }

                var result =  _context.Tasks.FirstOrDefault(n => n.Id == id);

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                _cache.SetString(cacheKey, JsonSerializer.Serialize(result), options);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Fetching Note  ", ex);
            }
        
            
        }


        public List<TaskM> GetTasks(int userid)
        {
            try
            {
                var cacheKey = GetNotesCacheKey(userid);
                var cacheData = _cache.GetString(cacheKey);
                if (cacheData != null)
                {
                    return JsonSerializer.Deserialize<List<TaskM>>(cacheData);
                }

                var result =  _context.Tasks.Where(n => n.UserId == userid).ToList();
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                _cache.SetString(cacheKey, JsonSerializer.Serialize(result), options);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in Fetching Note  ", ex);
            }

        }
        public TaskM UpdateTask(TaskM task)
        {
            _context.Update(task);
            _context.SaveChanges();
            ClearCache(task.UserId);
            return task;
        }


        public bool DeleteTask(int id)
        {
            var result = _context.Tasks.FirstOrDefault(n => n.Id == id);
            if(result != null)
            {
                _context.Tasks.Remove(result);
                _context.SaveChanges();
                ClearCache(result.UserId);
                return true;
            }

            return false;
        }


        public List<TaskM> GetAllTasks()
        {
            return _context.Tasks.OrderByDescending(t => t.CreatedAt).ToList();
        }
    }

}
