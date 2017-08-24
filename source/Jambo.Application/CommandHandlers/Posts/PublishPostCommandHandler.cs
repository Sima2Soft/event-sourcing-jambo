﻿using Jambo.Application.Commands;
using Jambo.Application.Commands.Blogs;
using Jambo.Application.Commands.Posts;
using Jambo.Domain.Aggregates.Blogs;
using Jambo.Domain.Aggregates.Posts;
using Jambo.Domain.ServiceBus;
using MediatR;
using System;
using System.Threading.Tasks;

namespace Jambo.Application.CommandHandlers.Posts
{
    public class PublishPostCommandHandler : IAsyncRequestHandler<PublishPostCommand>
    {
        private readonly IServiceBus _serviceBus;
        private readonly IPostReadOnlyRepository _postReadOnlyRepository;

        public PublishPostCommandHandler(
            IServiceBus serviceBus,
            IPostReadOnlyRepository postReadOnlyRepository)
        {
            _serviceBus = serviceBus ??
                throw new ArgumentNullException(nameof(serviceBus));
            _postReadOnlyRepository = postReadOnlyRepository ??
                throw new ArgumentNullException(nameof(postReadOnlyRepository));
        }

        public async Task Handle(PublishPostCommand message)
        {
            Post post = await _postReadOnlyRepository.GetPost(message.Id);
            post.Publish();

            await _serviceBus.Publish(post.GetEvents());
        }
    }
}