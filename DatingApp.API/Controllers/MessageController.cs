using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Extensions;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    public class MessageController : BaseApiController
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUserRepository userRepository;
         private readonly IMapper mapper;
        public MessageController(IUserRepository userRepository, IMessageRepository messageRepository       
        ,IMapper mapper)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
        }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)    
    {
        var username = User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("You cannot send messages to yourself");

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null) return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.Username,
            RecipientUsername = recipient.Username,
            Content = createMessageDto.Content
        };

        messageRepository.AddMessage(message);

        if(await messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)    
    {
        messageParams.Username=User.GetUsername();

        var messages=await messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages.CurrentPage,messages.PageSize,messages.TotalCount,messages.TotalPages);

        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername=User.GetUsername();

        return Ok(await messageRepository.GetMessageThread(currentUsername,username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username=User.GetUsername();
        var message=await messageRepository.GetMessage(id);

        if(message.Sender.Username!=username && message.Recipient.Username!=username)
            return Unauthorized();

        if(message.Sender.Username==username) message.SenderDeleted=true;

        if(message.Recipient.Username==username) message.RecipientDeleted=true;

        if(message.SenderDeleted && message.RecipientDeleted) 
            messageRepository.DeleteMessage(message);

        if(await messageRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting the message");
    }
}
}