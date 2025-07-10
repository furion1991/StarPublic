'use client'

import { useState } from 'react'

import { LatestOpenCasesFilter } from '@/features/cases'
import { LatestOpenCasesList } from '@/entities/cases'

import { useSignalrData } from '@/shared/hooks'

import classes from './LatestOpenCases.module.scss'

type Filter = 'top' | 'default'

export const LatestOpenCases = () => {
  const [selectedFilter, setSelectedFilter] = useState<Filter>('top')

  const {
    data: { latestOpenedCases, latestOpenedCasesTop }
  } = useSignalrData()

  return (
    <div className={classes.latestOpenCases}>
      <div className={classes.latestOpenCasesFilter}>
        <LatestOpenCasesFilter filter={selectedFilter} onFilterChange={setSelectedFilter} />
      </div>

      {selectedFilter === 'top' ? (
        <LatestOpenCasesList
          cases={latestOpenedCasesTop.map(({ userId, openedTimeStamp, item }) => ({
            key: `${userId}-${openedTimeStamp}`,
            image: item.image,
            name: item.name,
            rarity: item.rarity
          }))}
        />
      ) : null}

      {selectedFilter === 'default' ? (
        <LatestOpenCasesList
          cases={latestOpenedCases.map(({ userId, openedTimeStamp, item }) => ({
            key: `${userId}-${openedTimeStamp}`,
            image: item.image,
            name: item.name,
            rarity: item.rarity
          }))}
        />
      ) : null}
    </div>
  )
}
