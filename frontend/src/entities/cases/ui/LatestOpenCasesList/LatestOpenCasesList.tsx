'use client'

import Image from 'next/image'
import cn from 'classnames'
import { motion, AnimatePresence } from 'motion/react'
import { useEffect, useState } from 'react'

import { LootRarity } from '@/entities/loot'

import classes from './LatestOpenCasesList.module.scss'

type Case = {
  key: string
  image: string
  name: string
  rarity: LootRarity
}

type LatestOpenCasesListProps = {
  cases: Case[]
}

export const LatestOpenCasesList = ({ cases }: LatestOpenCasesListProps) => {
  const [isFirstRender, setFirstRender] = useState(true)

  useEffect(() => {
    if (isFirstRender) {
      setFirstRender(false)
    }
  }, [isFirstRender])

  return (
    <ul className={classes.latestOpenCasesList}>
      <AnimatePresence mode='popLayout' initial={false}>
        {cases.map(({ key, rarity, image, name }) => {
          return (
            <motion.li
              key={key}
              layout
              initial={isFirstRender ? {} : { x: -169 }}
              animate={isFirstRender ? {} : { x: 0 }}
              exit={isFirstRender ? {} : { x: 169 }}
              transition={{ ease: 'easeInOut', duration: 0.3 }}
              className={cn(classes.lastOpenedCaseItem, {
                [classes.common]: rarity === LootRarity.COMMON,
                [classes.rare]: rarity === LootRarity.RARE,
                [classes.superRare]: rarity === LootRarity.SUPER_RARE,
                [classes.epic]: rarity === LootRarity.EPIC,
                [classes.mythical]: rarity === LootRarity.MYTHICAL,
                [classes.legendary]: rarity === LootRarity.LEGENDARY
              })}
            >
              <div className={classes.image}>
                <Image src={image} width={54} height={54} alt={name} />
              </div>

              <p>{name}</p>

              <div className={cn(classes.angleCut, classes.left)} />
              <div className={cn(classes.angleCut, classes.right)} />
            </motion.li>
          )
        })}
      </AnimatePresence>
    </ul>
  )
}
