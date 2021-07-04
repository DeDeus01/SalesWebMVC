using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesWebMVC.Models;
using Microsoft.EntityFrameworkCore;
using SalesWebMVC.Services.Exceptions;

namespace SalesWebMVC.Services
{
    public class SellerService
    {
        private readonly SalesWebMVCContext _context; //objeto do banco, entity framework

        public SellerService(SalesWebMVCContext context) //injeção de dependencia
        {
            _context = context;
        }

        public List<Seller> FindAll()
        {
            return _context.Seller.ToList(); //consulta a fonte de dados ta tabela seller e converter em uma lista.
        }
        //o metodo de serviço tem a funcao de consultar o banco  e retornar a lista de objetos, diferentemente do model onde
        //trabalha exclusivamente a um objeto especifico, dessa forma existe uma camada responsavel exatamente para este fim.
            
        public void Insert(Seller obj)
        {
            _context.Add(obj);          //insere no banco usando o metodo Add e depois comita com save changes.
            _context.SaveChanges();
        }

        public Seller FindById(int id)
        {//o metodo include serve para fazer o join com a tabela department e ser anexado ao objeto seller
            return _context.Seller.Include(obj => obj. Department).FirstOrDefault(obj => obj.Id == id);
        }

        public void Update(Seller obj)
        {
            if(!_context.Seller.Any(x => x.Id == obj.Id))
            {
                throw new NotFoundException("Id not found");
            }

            try
            {
                _context.Update(obj);
                _context.SaveChanges();
            }catch(DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }

        public void Remove(int id)
        {
            var obj = _context.Seller.Find(id);
            _context.Seller.Remove(obj);
            _context.SaveChanges();
        }
    }
}
