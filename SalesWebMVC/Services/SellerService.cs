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

        //uma task asyncrona de list seller 
        public async Task<List<Seller>> FindAllAsync()
        {
            return await _context.Seller.ToListAsync(); //consulta a fonte de dados ta tabela seller e converter em uma lista.
        }
        //o metodo de serviço tem a funcao de consultar o banco  e retornar a lista de objetos, diferentemente do model onde
        //trabalha exclusivamente a um objeto especifico, dessa forma existe uma camada responsavel exatamente para este fim.
            

        public async Task InsertAsync(Seller obj)
        {
            _context.Add(obj);          //insere no banco usando o metodo Add e depois comita com save changes.
            await _context.SaveChangesAsync();
        }


        public async Task<Seller> FindByIdAsync(int id)
        {//o metodo include serve para fazer o join com a tabela department e ser anexado ao objeto seller
            return await _context.Seller.Include(obj => obj. Department).FirstOrDefaultAsync(obj => obj.Id == id);
        }


        public async Task UpdateAsync(Seller obj)
        {
            bool hasAny = await _context.Seller.AnyAsync(x => x.Id == obj.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id not found");
            }

            try
            {
                _context.Update(obj);
                await _context.SaveChangesAsync();
            }catch(DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }

        public async Task RemoveAsync(int id)
        {
            var obj = await _context.Seller.FindAsync(id);
            _context.Seller.Remove(obj);
            await _context.SaveChangesAsync();
        }
    }
}
