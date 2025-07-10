import cn from 'classnames'

import classes from './SuperBonusWord.module.scss'

type SuperBonusWordProps = {
  word: string
  activatedLetters: string[]
}

export const SuperBonusWord = ({ word, activatedLetters }: SuperBonusWordProps) => {
  return (
    <ul className={classes.superBonusWord}>
      {word.split('').map((letter, idx) => {
        return (
          <li
            key={`${letter}-${idx}`}
            className={cn(classes.letterField, {
              [classes.activated]: activatedLetters
                .map((letter) => letter.toLowerCase())
                .includes(letter.toLowerCase())
            })}
          >
            <span>{letter.toUpperCase()}</span>
          </li>
        )
      })}
    </ul>
  )
}
