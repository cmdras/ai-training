import Anthropic from '@anthropic-ai/sdk';
import dotenv from 'dotenv'; 

const client = new Anthropic({
  apiKey: process.env['ANTHROPIC_API_KEY']
});

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


async function AskClaude(question: string[]) {
  const message = await client.messages.create({
    max_tokens: 1024,
    messages: [
        { role: 'user', 
            content: question.join(" ") 
        }
    ],
    model: 'claude-opus-4-6',
  });

  return formatOutput(message.content);
}

console.log(await AskClaude(question));