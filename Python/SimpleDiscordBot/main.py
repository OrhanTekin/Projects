import os
import discord
import random
from discord.ext import commands
from discord.utils import get
from dotenv import load_dotenv
#import asyncio


discord_channel_id = 659980885076082719   #channel id
discord_role_member_id = 659983224105205762  #member role id

intents = discord.Intents.all()
intents.typing = False
intents.message_content = True

client = commands.Bot(command_prefix='!', intents=intents)

load_dotenv()


@client.event
async def on_message(message):
    if message.author == client.user:
        return

    #if message.guild is None:
    #    await message.channel.send("In DMs this currently doesnt work!")
    #    return

    await client.process_commands(message)


@client.event
async def on_member_join(member:discord.Member):
    channel = get(member.guild.text_channels, id=discord_channel_id)
    #add default role for new members here
    role = get(member.guild.roles, id=discord_role_member_id)  # Member role
    if role:
        await member.add_roles(role)

    if channel:
        await channel.send(f"Welcome to the server, {member.mention}! 🎉 You've been assigned the {role.name} role.")


@client.command(name='inv')
async def invite(message, user:discord.User ,channel:discord.TextChannel = None):
    if channel is None:  # No channel tagged, create invite for current channel
        invitelink = await message.channel.create_invite(max_uses=1,unique=True)
    else:
        invitelink = await channel.create_invite(max_uses=1,unique=True)

    await user.send(invitelink)


@client.command(name='flip')
async def flip_coin(message):
    flip = random.randint(0,1)
    coin = 'Heads'
    if flip == 0:
        coin = 'Tails'
    await message.channel.send(coin + "!")   


@client.event
async def on_connect():
    print("Bot connected!")


TOKEN = os.getenv('TOKEN')
client.run(TOKEN)