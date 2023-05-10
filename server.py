import openai
from flask import Flask, request, jsonify



app = Flask(__name__)

openai.api_key = "REDACTED"
message_history = []

@app.route('/Chat-GPT/<string:query>', methods=['GET'])
def chatGPT(query):
    message_history.append({"role":"user", "content": query})
    completion = openai.ChatCompletion.create(
    model="gpt-3.5-turbo",
    messages=message_history
    )
    response = completion["choices"][0].message.content
    message_history.append({"role":"assistant", "content": response})
    return jsonify(response)

if __name__ == '__main__':
    app.run(debug=True)
