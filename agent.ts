import Anthropic from '@anthropic-ai/sdk';
import dotenv from 'dotenv'; 
import * as readline from 'readline/promises';

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

const client = new Anthropic({
  apiKey: process.env['ANTHROPIC_API_KEY']
});

let messageHistory: string[] = [];

const question = process.argv.slice(2);

function formatOutput(output: Anthropic.Messages.ContentBlock[]) {
  return output
    .map((block) => {
      if (block.type === "text") {
        return block.text;
      }
      return block.type;
    })
    .join("\n");
}

async function AskClaude(question: string) {
  const message = await client.messages.create({
    max_tokens: 1024,
    messages: [
        { role: 'user', 
            content: question 
        }
    ],
    model: 'claude-opus-4-6',
  });

  return formatOutput(message.content);
}

async function ChatWithClaude() {
    console.log("Hi how can I help?")
    console.log("\n");
    let greeting = ">:";
    
    let userInput = await rl.question(greeting)
    messageHistory.push(greeting);
    messageHistory.push(userInput);

    let response = await AskClaude(messageHistory.join(" "));
    messageHistory.push(response);
    console.log(response);
}

while (true){
    await ChatWithClaude()
}