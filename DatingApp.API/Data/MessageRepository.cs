using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;
        public MessageRepository(DataContext dataContext, IMapper mapper)
        {
            this.mapper = mapper;
            this.dataContext = dataContext;
        }

        public void AddMessage(Message message)
        {
            dataContext.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            dataContext.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await dataContext.Messages
            .Include(m=>m.Sender)
            .Include(m=>m.Recipient)
            .SingleOrDefaultAsync(m=>m.Id==id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = dataContext.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

            switch (messageParams.Container)
            {
                case "Inbox":
                    query = query.Where(u => u.RecipientUsername == messageParams.Username && !u.RecipientDeleted);
                    break;
                case "Outbox":
                    query = query.Where(u => u.SenderUsername == messageParams.Username && !u.SenderDeleted);
                    break;
                default:
                    query = query.Where(u => u.RecipientUsername == messageParams.Username
                      && u.MessageRead == null && !u.RecipientDeleted);
                    break;
            }
            
            var messages=query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize);

        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages=await dataContext.Messages
            .Include(m=>m.Sender).ThenInclude(s=>s.Photos)
            .Include(m=>m.Recipient).ThenInclude(s=>s.Photos)
            .Where(m=>m.Recipient.Username==currentUsername && !m.RecipientDeleted
                && m.Sender.Username==recipientUsername
                || m.Recipient.Username==recipientUsername
                && m.Sender.Username==currentUsername && !m.SenderDeleted
            )
            .OrderBy(m=>m.MessageSent)
            .ToListAsync();

            var unreadMessages=messages.Where(m=>m.MessageRead==null && m.Recipient.Username==currentUsername);
            if(unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.MessageRead=DateTime.Now;
                }
                await dataContext.SaveChangesAsync();
            }
            

            return mapper.Map<IEnumerable<MessageDto>>(messages);

        }

        public async Task<bool> SaveAllAsync()
        {
            return await dataContext.SaveChangesAsync() > 0;
        }
    }
}