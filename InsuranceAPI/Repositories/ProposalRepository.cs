﻿using InsuranceAPI.Context;
using InsuranceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceAPI.Repositories
{
    public class ProposalRepository : Repository<int, Proposal>
    {
        public ProposalRepository(InsuranceManagementContext context) : base(context) { }

        public override async Task<Proposal> GetById(int key)
        {
            var proposal = await _context.Proposals
                .Include(p => p.Client)
                .Include(p => p.Vehicle)
                .FirstOrDefaultAsync(p => p.ProposalId == key);

            if (proposal == null)
                throw new Exception($"Proposal with ID {key} not found");

            return proposal;
        }

        public override async Task<IEnumerable<Proposal>> GetAll()
        {
            var proposals = await _context.Proposals
                .Include(p => p.Client)
                .Include(p => p.Vehicle)
                .ToListAsync();

            if (proposals.Count == 0)
                throw new Exception("No proposals found");

            return proposals;
        }
    }
}
