# Базовый образ
FROM node:18-alpine

# Указываем рабочую директорию
WORKDIR /app

# Копируем только package.json и устанавливаем зависимости
COPY package*.json ./
RUN npm install

# Копируем весь проект
COPY . .

# Открываем порт для dev-сервера
EXPOSE 3000

# Запускаем dev-сервер
CMD ["npm", "run", "dev"]
