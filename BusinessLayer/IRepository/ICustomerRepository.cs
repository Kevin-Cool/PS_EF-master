﻿using BusinessLayer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.IRepository
{
    public interface ICustomerRepository
    {
        public Klant Add(Klant a);
        public Klant GetByID(long id);
        public List<Klant> GetAll();
        public void Delete(long id);
        public void DeleteAll();
        public void Update(Klant a);
        public bool Exist(Klant a, bool ignoreId = false);
    }
}
