'use client'

import { useState } from 'react'

import { LatestOpenCasesFilter } from '@/features/cases'
import { LatestOpenCasesList } from '@/entities/cases'

import { useUserStatistics } from '@/shared/hooks'

import classes from './LatestOpenCases.module.scss'

type Filter = 'top' | 'default'

export const LatestOpenCases = () => {
  const [selectedFilter, setSelectedFilter] = useState<Filter>('top')

  const {
    data: { latestOpenedCases, latestOpenedCasesTop }
  } = useUserStatistics()

  return (
    <div className={classes.latestOpenCases}>
      <div className={classes.latestOpenCasesFilter}>
        <LatestOpenCasesFilter filter={selectedFilter} onFilterChange={setSelectedFilter} />
      </div>

      {selectedFilter === 'top' ? (
        <LatestOpenCasesList
          cases={latestOpenedCasesTop.map(({ userId, openedTimeStamp, item }, idx) => ({
            key: `${userId}-${openedTimeStamp}-${idx}`,
            image: item.image,
            name: item.name,
            rarity: item.rarity
          }))}
        />
      ) : null}

      {selectedFilter === 'default' ? (
        <LatestOpenCasesList
          cases={latestOpenedCases.map(({ userId, openedTimeStamp, item }, idx) => ({
            key: `${userId}-${openedTimeStamp}-${idx}`,
            image: item.image,
            name: item.name,
            rarity: item.rarity
          }))}
        />
      ) : null}
    </div>
  )
}
