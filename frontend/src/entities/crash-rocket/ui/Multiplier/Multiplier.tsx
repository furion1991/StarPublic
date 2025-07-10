'use client'

import cn from 'classnames'

import classes from './Multiplier.module.scss'

enum LootRarity {
  COMMON = 0,
  RARE = 1,
  SUPER_RARE = 2,
  EPIC = 3
}

type MultiplierProps = {
  rarity: LootRarity
  multiplier: number
}

export const Multiplier = ({ multiplier, rarity }: MultiplierProps) => {
  return (
    <div className={classes.multiplier}>
      <p className={classes.label}>Множитель</p>

      <p className={classes.value}>{multiplier} X</p>

      <svg
        className={classes.bg}
        xmlns='http://www.w3.org/2000/svg'
        width='254'
        height='121'
        viewBox='0 0 254 121'
        fill='none'
      >
        <path
          d='M24.753 120.317C82.7705 115.269 176.559 107.176 216.967 103.919C228.125 103.019 239.08 94.4437 242.494 83.7829L252.863 51.4065C257.082 38.2345 248.243 24.4833 234.509 22.8503L44.7046 0.283347C33.7217 -1.02247 23.4768 6.02862 20.7741 16.7535L1.5575 93.0099C-2.1401 107.683 9.67819 121.628 24.753 120.317Z'
          fill='url(#paint0_linear_33_16423)'
        />
        <defs>
          <linearGradient
            id='paint0_linear_33_16423'
            x1='29.5'
            y1='49.5'
            x2='236.5'
            y2='69.5'
            gradientUnits='userSpaceOnUse'
          >
            <stop
              className={cn(classes.bgColor, {
                [classes.purple]: rarity === 0,
                [classes.green]: rarity === 1,
                [classes.yellow]: rarity === 2,
                [classes.red]: rarity === 3
              })}
            />
            <stop offset='1' stopColor='#1B205A' stopOpacity='0.1' />
          </linearGradient>
        </defs>
      </svg>
    </div>
  )
}
