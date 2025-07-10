export const splitByCapitalLetter = (str: string) => {
  const splitted = str.match(/[A-Z][a-z]+/g)

  if (splitted) return splitted.join(' ')

  return str
}
