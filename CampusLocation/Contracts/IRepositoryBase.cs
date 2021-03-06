﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLocation.Contracts
{
    public interface IRepositoryBase<T> where T : class 
    {
        Task<IList<T>> GetAll();
        Task<T> FindById(int id);
        Task<bool> doesExist(int id);
        Task<bool> Create(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
        Task<bool> Save();
    }
}
