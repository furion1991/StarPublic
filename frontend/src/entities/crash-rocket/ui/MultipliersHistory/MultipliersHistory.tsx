'use client'

import cn from 'classnames'
import { useEffect, useState } from 'react'

import classes from './MultipliersHistory.module.scss'

enum LootRarity {
  COMMON = 0,
  RARE = 1,
  SUPER_RARE = 2,
  EPIC = 3,
  MYTHICAL = 4,
  LEGENDARY = 5
}

export const MultipliersHistory = () => {
  const [multipliersValues, setMultipliersValues] = useState<number[]>([])
  const [multipliersRarities, setMultipliersRarities] = useState<number[]>([])

  useEffect(() => {
    setMultipliersValues(
      Array.from({ length: 20 })
        .fill(null)
        .map(() => Number(randomIntFromInterval(1, 6).toFixed(2)))
    )

    setMultipliersRarities(
      Array.from({ length: 20 })
        .fill(null)
        .map(() => Math.floor(randomIntFromInterval(0, 5)))
    )
  }, [])

  function randomIntFromInterval(min: number, max: number) {
    return Math.random() * (max - min + 1) + min
  }

  const multipliers = Array.from({ length: 20 })
    .fill(null)
    .map((_, idx) => ({
      id: idx,
      value: multipliersValues[idx],
      rarity: multipliersRarities[idx]
    }))

  return (
    <ul className={classes.multipliersHistory}>
      {multipliers.map(({ id, value, rarity }) => {
        return (
          <li
            key={id}
            className={cn({
              [classes.common]: rarity === LootRarity.COMMON,
              [classes.rare]: rarity === LootRarity.RARE,
              [classes.superRare]: rarity === LootRarity.SUPER_RARE,
              [classes.epic]: rarity === LootRarity.EPIC,
              [classes.mythical]: rarity === LootRarity.MYTHICAL,
              [classes.legendary]: rarity === LootRarity.LEGENDARY
            })}
          >
            <span>{value} x</span>
          </li>
        )
      })}
    </ul>
  )
}
