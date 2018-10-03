using ApplicationCore.Interfaces;
using System;

namespace ApplicationCore.Services
{
    public class UniqueIDGenerator : IUniqueIDGenerator
    {
        public long GetNextId()
        {
            long id = DateTime.Now.Ticks;
            return id;
        }
    }
}
