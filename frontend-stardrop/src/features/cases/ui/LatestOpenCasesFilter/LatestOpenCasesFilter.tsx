'use client'

import Image from 'next/image'
import cn from 'classnames'

import classes from './LatestOpenCasesFilter.module.scss'

type Filter = 'top' | 'default'

type LatestOpenCasesFilterProps = {
  filter: Filter
  onFilterChange: (filter: Filter) => void
}

export const LatestOpenCasesFilter = ({ filter, onFilterChange }: LatestOpenCasesFilterProps) => {
  return (
    <div className={classes.latestOpenCasesFilter}>
      <div
        className={cn(classes.btnWrapper, {
          [classes.btnActive]: filter === 'top'
        })}
        onClick={() => {
          onFilterChange('top')
        }}
      >
        <button>
          <Image src='/icons/crown.svg' width={27} height={27} alt='Корона' />
          <span>Топ</span>
        </button>
      </div>

      <div
        className={cn(classes.btnWrapper, {
          [classes.btnActive]: filter === 'default'
        })}
        onClick={() => {
          onFilterChange('default')
        }}
      >
        <button>
          <Image src='/icons/layout-grid.svg' width={16} height={16} alt='Сетка' />
          <span>Все</span>
        </button>
      </div>
    </div>
  )
}
