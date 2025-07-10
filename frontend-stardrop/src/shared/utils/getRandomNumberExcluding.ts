export const getRandomNumberExcluding = (excludedNumbers: number[]) => {
  const numbers = [0, 1, 2, 3, 4, 5, 6]
  const filteredNumbers = numbers.filter((number) => !excludedNumbers.includes(number))
  const randomIndex = Math.floor(Math.random() * filteredNumbers.length)

  return filteredNumbers[randomIndex]
}
